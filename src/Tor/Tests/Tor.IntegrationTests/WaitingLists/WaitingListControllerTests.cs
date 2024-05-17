using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using Tor.Contracts.WaitingList;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.IntegrationTests.WaitingLists;
public class WaitingListControllerTests : BaseIntegrationTest
{
    public WaitingListControllerTests(IntegrationTestWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task JoinWaitingList_WhenValidRequest_ShouldJoinWaitingList()
    {
        var staffMember = await TestUtils.SetupStaffMember(Context);
        var client = await TestUtils.SetupClient(Context);
        var command = Fakers.WaitingLists.JoinWaitingListCommandFaker.Generate() with
        {
            StaffMemberId = staffMember.Id,
            ClientId = client.Id,
        };

        var result = await Client.PostAsJsonAsync(WaitingListControllerConstants.JoinWaitingListUri, command);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var waitingListFromDb = await Context.WaitingLists
            .Include(x => x.Clients)
            .SingleOrDefaultAsync(x => x.AtDate == command.AtDate && x.StaffMemberId == staffMember.Id);
        waitingListFromDb.Should().NotBeNull();
        waitingListFromDb!.Clients.Should().Contain(x => x.Id == client.Id);
    }

    [Fact]
    public async Task GetByClient_WhenValidRequest_ShouldGetClientWaitingLists()
    {
        var staffMember = await TestUtils.SetupStaffMember(Context);
        var client = await TestUtils.SetupClient(Context);
        var waitingLists = Fakers.WaitingLists.WaitingListFaker.Generate(5);
        waitingLists.ForEach(x => x.StaffMemberId = staffMember.Id);
        client.WaitingLists.AddRange(waitingLists);
        await Context.SaveChangesAsync();
        string requestUri = $"{WaitingListControllerConstants.GetByClientUri}?clientId={client.Id}";

        var result = await Client.GetFromJsonAsync<GetWaitingListsResponse>(requestUri);

        result.Should().NotBeNull();
        waitingLists.Should().BeEquivalentTo(result!.WaitingLists, cfg => cfg.ExcludingMissingMembers());
        result!.WaitingLists.Should().AllSatisfy(x => x.StaffMemberId.Should().Be(staffMember.Id));
    }
}
