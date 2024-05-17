using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.WaitingLists.Commands.JoinWaitingList;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.WaitingLists;

public class JoinWaitingListCommandHandlerTests : UnitTestBase
{
    private readonly JoinWaitingListCommandHandler _sut;

    public JoinWaitingListCommandHandlerTests()
        : base()
    {
        _sut = new JoinWaitingListCommandHandler(Context);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_WhenValidCommand_ShouldJoinWaitingList(bool createWaitingList)
    {
        var staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        var client = Fakers.Clients.ClientFaker.Generate();
        Context.StaffMembers.Add(staffMember);
        Context.Clients.Add(client);
        if (createWaitingList)
        {
            var waitingList = Fakers.WaitingLists.WaitingListFaker.Generate();
            Context.WaitingLists.Add(waitingList);
        }
        await Context.SaveChangesAsync();
        var command = Fakers.WaitingLists.JoinWaitingListCommandFaker.Generate() with { StaffMemberId = staffMember.Id, ClientId = client.Id };

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        var waitingListFromDb = await Context.WaitingLists
            .Include(x => x.Clients)
            .SingleOrDefaultAsync(x => x.StaffMemberId == staffMember.Id && x.AtDate == command.AtDate);
        waitingListFromDb.Should().NotBeNull();
        waitingListFromDb!.Clients.Should().Contain(client);
    }

    [Fact]
    public async Task Handle_WhenServiceDoesntExist_ShouldReturnNotFoundError()
    {
        var client = Fakers.Clients.ClientFaker.Generate();
        Context.Clients.Add(client);
        await Context.SaveChangesAsync();
        var command = Fakers.WaitingLists.JoinWaitingListCommandFaker.Generate() with { ClientId = client.Id };

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find staff member with id {command.StaffMemberId}"));
    }

    [Fact]
    public async Task Handle_WhenClientDoesntExist_ShouldReturnNotFoundError()
    {
        var staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        Context.StaffMembers.Add(staffMember);
        await Context.SaveChangesAsync();
        var command = Fakers.WaitingLists.JoinWaitingListCommandFaker.Generate() with { StaffMemberId = staffMember.Id };

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find client with id {command.ClientId}"));
    }
}
