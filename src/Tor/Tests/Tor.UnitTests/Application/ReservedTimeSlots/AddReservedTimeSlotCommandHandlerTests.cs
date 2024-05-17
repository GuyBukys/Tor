using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.ReservedTimeSlots.Commands.Add;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.ReservedTimeSlots;

public class AddReservedTimeSlotCommandHandlerTests : UnitTestBase
{
    private readonly AddReservedTimeSlotCommandHandler _sut;

    public AddReservedTimeSlotCommandHandlerTests()
        : base()
    {
        _sut = new AddReservedTimeSlotCommandHandler(Context);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldAddReservedTimeSlot()
    {
        StaffMember staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        await Context.StaffMembers.AddAsync(staffMember);
        await Context.SaveChangesAsync();
        var command = Fakers.ReservedTimeSlots.AddReservedTimeSlotCommandFaker.Generate() with
        {
            StaffMemberId = staffMember.Id,
        };

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        var reservedTimeSlotFromDb = await Context.ReservedTimeSlots.SingleOrDefaultAsync(x => x.StaffMemberId == staffMember.Id);
        reservedTimeSlotFromDb.Should().NotBeNull();
        reservedTimeSlotFromDb.Should().BeEquivalentTo(command, cfg => cfg.ExcludingMissingMembers());
    }

    [Fact]
    public async Task Handle_WhenStaffMemberDoesntExist_ShouldReturnNotFound()
    {
        var command = Fakers.ReservedTimeSlots.AddReservedTimeSlotCommandFaker.Generate();

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find staff member with id {command.StaffMemberId}"));
    }
}
