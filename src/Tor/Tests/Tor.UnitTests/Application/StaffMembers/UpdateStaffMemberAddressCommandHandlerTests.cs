using FluentAssertions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.StaffMembers.Commands.UpdateStaffMemberAddress;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.StaffMembers;

public class UpdateStaffMemberAddressCommandHandlerTests : UnitTestBase
{
    private readonly UpdateStaffMemberAddressCommandHandler _sut;

    public UpdateStaffMemberAddressCommandHandlerTests()
        : base()
    {
        _sut = new UpdateStaffMemberAddressCommandHandler(Context);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldUpdateAddress()
    {
        StaffMember staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        await Context.StaffMembers.AddAsync(staffMember);
        await Context.SaveChangesAsync();
        var command = Fakers.StaffMembers.UpdateStaffMemberAddressCommandFaker.Generate() with { StaffMemberId = staffMember.Id };

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await Context.Entry(staffMember).ReloadAsync();
        staffMember.Address.Should().Be(command.Address);
    }

    [Fact]
    public async Task Handle_WhenStaffMemberDoesntExist_ShouldReturnNotFoundError()
    {
        var command = Fakers.StaffMembers.UpdateStaffMemberAddressCommandFaker.Generate();

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find staff member with id {command.StaffMemberId}"));
    }
}
