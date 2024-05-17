using FluentAssertions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.StaffMembers.Commands.UpdateWeeklySchedule;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Businesses;

public class UpdateWeeklyScheduleCommandHandlerTests : UnitTestBase
{
    private readonly UpdateStaffMemberWeeklyScheduleCommandHandler _sut;

    public UpdateWeeklyScheduleCommandHandlerTests()
        : base()
    {
        _sut = new UpdateStaffMemberWeeklyScheduleCommandHandler(Context);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldUpdateWeeklySchedule()
    {
        StaffMember staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        await Context.StaffMembers.AddAsync(staffMember);
        await Context.SaveChangesAsync();
        var command = Fakers.StaffMembers.UpdateStaffMemberWeeklyScheduleCommandFaker.Generate() with { StaffMemberId = staffMember.Id };

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await Context.Entry(staffMember).ReloadAsync();
        staffMember.WeeklySchedule.Should().Be(command.WeeklySchedule);
    }

    [Fact]
    public async Task Handle_WhenStaffMemberDoesntExist_ShouldReturnNotFoundError()
    {
        var command = Fakers.StaffMembers.UpdateStaffMemberWeeklyScheduleCommandFaker.Generate();

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find staff member with id {command.StaffMemberId}"));
    }
}
