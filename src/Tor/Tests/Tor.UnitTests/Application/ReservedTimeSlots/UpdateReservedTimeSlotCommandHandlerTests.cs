using FluentAssertions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.ReservedTimeSlots.Commands.Update;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.ReservedTimeSlots;

public class UpdateReservedTimeSlotCommandHandlerTests : UnitTestBase
{
    private readonly UpdateReservedTimeSlotCommandHandler _sut;

    public UpdateReservedTimeSlotCommandHandlerTests()
        : base()
    {
        _sut = new UpdateReservedTimeSlotCommandHandler(Context);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldUpdateReservedTimeSlot()
    {
        StaffMember staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        await Context.StaffMembers.AddAsync(staffMember);
        await Context.SaveChangesAsync();
        ReservedTimeSlot reservedTimeSlot = Fakers.ReservedTimeSlots.ReservedTimeSlotFaker.Generate();
        reservedTimeSlot.StaffMemberId = staffMember.Id;
        await Context.ReservedTimeSlots.AddAsync(reservedTimeSlot);
        await Context.SaveChangesAsync();
        var command = Fakers.ReservedTimeSlots.UpdateReservedTimeSlotCommandFaker.Generate() with
        {
            Id = reservedTimeSlot.Id,
        };

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await Context.Entry(reservedTimeSlot).ReloadAsync();
        reservedTimeSlot.Should().BeEquivalentTo(command, cfg => cfg.ExcludingMissingMembers());
    }

    [Fact]
    public async Task Handle_WhenReservedTimeSlotDoesntExist_ShouldReturnNotFound()
    {
        var command = Fakers.ReservedTimeSlots.UpdateReservedTimeSlotCommandFaker.Generate();

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find reserved time slot with id {command.Id}"));
    }
}
