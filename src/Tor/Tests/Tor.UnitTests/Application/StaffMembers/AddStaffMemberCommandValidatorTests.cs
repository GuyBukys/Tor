using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.StaffMembers.Commands.AddStaffMember;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.StaffMembers;

public class AddStaffMemberCommandValidatorTests
{
    private readonly AddStaffMemberCommandValidator _sut;

    public AddStaffMemberCommandValidatorTests()
    {
        _sut = new AddStaffMemberCommandValidator();
    }

    [Fact]
    public async Task Validate_WhenValidCommand_ShouldReturnTrue()
    {
        var command = Fakers.StaffMembers.AddStaffMemberCommandFaker.Generate();

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var command = Fakers.StaffMembers.AddStaffMemberCommandFaker.Generate();
        var invalidCommand = command with
        {
            BusinessId = Guid.Empty,
            Name = null!,
            Email = null!,
            PhoneNumber = null!,
            Services = null!,
            UserId = Guid.Empty,
        };

        var result = await _sut.TestValidateAsync(invalidCommand);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.BusinessId);
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Email);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
        result.ShouldHaveValidationErrorFor(x => x.Services);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public async Task Validate_WhenOneOfTheServicesIsNull_ShouldReturnFalse()
    {
        var command = Fakers.StaffMembers.AddStaffMemberCommandFaker.Generate();
        command.Services[0] = null!;

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor("Services[0]");
    }

    [Fact]
    public async Task Validate_WhenOneOfTheServicesIsInvalid_ShouldReturnFalse()
    {
        var command = Fakers.StaffMembers.AddStaffMemberCommandFaker.Generate();
        command.Services[0] = new AddStaffMemberServiceCommand(null!, null!, null!);
        command.Services[1] = new AddStaffMemberServiceCommand(string.Empty, null!, new List<Duration>());

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor("Services[0].Name");
        result.ShouldHaveValidationErrorFor("Services[0].Amount");
        result.ShouldHaveValidationErrorFor("Services[0].Durations");
        result.ShouldHaveValidationErrorFor("Services[1].Name");
        result.ShouldHaveValidationErrorFor("Services[1].Amount");
        result.ShouldHaveValidationErrorFor("Services[1].Durations");
    }

    [Theory]
    [InlineData("not a valid email")]
    [InlineData("")]
    [InlineData("test.test.com")]
    public async Task Validate_WhenEmailIsInvalid_ShouldReturnFalse(string email)
    {
        var command = Fakers.StaffMembers.AddStaffMemberCommandFaker.Generate();
        var invalidCommand = command with
        {
            Email = email,
        };

        var result = await _sut.TestValidateAsync(invalidCommand);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
}
