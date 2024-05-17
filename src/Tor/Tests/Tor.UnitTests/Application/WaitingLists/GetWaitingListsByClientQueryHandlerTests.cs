using FluentAssertions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.WaitingLists.Queries.GetByClient;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.WaitingLists;

public class GetWaitingListsByClientQueryHandlerTests : UnitTestBase
{
    private readonly GetWaitingListsByClientQueryHandler _sut;

    public GetWaitingListsByClientQueryHandlerTests()
        : base()
    {
        _sut = new GetWaitingListsByClientQueryHandler(Context);
    }

    [Fact]
    public async Task Handle_WhenValidQuery_ShouldGetClientWaitingLists()
    {
        var staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        staffMember.Business = Fakers.Businesses.BusinessFaker.Generate();
        await Context.StaffMembers.AddAsync(staffMember);
        var client = Fakers.Clients.ClientFaker.Generate();
        await Context.Clients.AddAsync(client);
        var waitingLists = Fakers.WaitingLists.WaitingListFaker.Generate(10);
        waitingLists.ForEach(x => x.StaffMemberId = staffMember.Id);
        client.WaitingLists.AddRange(waitingLists);
        await Context.SaveChangesAsync();
        var query = new GetWaitingListsByClientQuery(client.Id);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        waitingLists.Should().BeEquivalentTo(result.Value, cfg => cfg.ExcludingMissingMembers());
    }

    [Fact]
    public async Task Handle_WhenClientDoesntExist_ShouldReturnNotFoundError()
    {
        var query = new GetWaitingListsByClientQuery(Guid.NewGuid());

        var result = await _sut.Handle(query, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find client with id {query.ClientId}"));
    }

    [Fact]
    public async Task Handle_WhenSomeWaitingListsAreNotCurrentOrFutureDate_ShouldGetRemainingWaitingLists()
    {
        var staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        staffMember.Business = Fakers.Businesses.BusinessFaker.Generate();
        await Context.StaffMembers.AddAsync(staffMember);
        var client = Fakers.Clients.ClientFaker.Generate();
        await Context.Clients.AddAsync(client);
        var waitingLists = Fakers.WaitingLists.WaitingListFaker.Generate(2);
        waitingLists.ForEach(x => x.StaffMemberId = staffMember.Id);
        waitingLists.First().AtDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        client.WaitingLists.AddRange(waitingLists);
        await Context.SaveChangesAsync();
        var query = new GetWaitingListsByClientQuery(client.Id);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.Should().Satisfy(x => x.AtDate == waitingLists.Last().AtDate);
    }
}
