using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.Businesses.Commands.UpdateAddress;
using Tor.Application.StaffMembers.Commands.UpdateStaffMemberAddress;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.StaffMembers;

public class UpdateStaffMemberAddressCommandValidatorTests
{
    private readonly UpdateStaffMemberAddressCommandValidator _sut;

    public UpdateStaffMemberAddressCommandValidatorTests()
    {
        _sut = new UpdateStaffMemberAddressCommandValidator();
    }

    [Fact]
    public async Task Validate_WhenValidCommand_ShouldReturnTrue()
    {
        var command = Fakers.StaffMembers.UpdateStaffMemberAddressCommandFaker.Generate();

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var command = new UpdateStaffMemberAddressCommand(Guid.Empty, null!);

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.StaffMemberId);
        result.ShouldHaveValidationErrorFor(x => x.Address);
    }
}
