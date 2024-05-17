using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.Businesses.Commands.UpdatePersonalDetails;
using Tor.Domain.Common.ValueObjects;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Businesses;

public class UpdateBusinessPersonalDetailsCommandValidatorTests
{
    private readonly UpdateBusinessPersonalDetailsCommandValidator _sut;

    public UpdateBusinessPersonalDetailsCommandValidatorTests()
    {
        _sut = new UpdateBusinessPersonalDetailsCommandValidator();
    }

    [Fact]
    public async Task Validate_WhenValidCommand_ShouldReturnTrue()
    {
        var command = Fakers.Businesses.UpdateBusinessPersonalDetailsCommandFaker.Generate();

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var command = Fakers.Businesses.UpdateBusinessPersonalDetailsCommandFaker.Generate();
        var invalidCommand = Fakers.Businesses.UpdateBusinessPersonalDetailsCommandFaker.Generate() with
        {
            BusinessId = Guid.Empty,
            Name = null!,
            Description = null!,
            Email = null!,
            PhoneNumbers = null!,
        };

        var result = await _sut.TestValidateAsync(invalidCommand);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.BusinessId);
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Description);
        result.ShouldHaveValidationErrorFor(x => x.Email);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumbers);
    }

    [Fact]
    public async Task Validate_WhenEmailIsInvalid_ShouldReturnFalse()
    {
        var command = Fakers.Businesses.UpdateBusinessPersonalDetailsCommandFaker.Generate();
        var invalidCommand = Fakers.Businesses.UpdateBusinessPersonalDetailsCommandFaker.Generate() with
        {
            Email = "not a valid email",
        };

        var result = await _sut.TestValidateAsync(invalidCommand);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task Validate_WhenPhoneNumbersIsEmpty_ShouldReturnFalse()
    {
        var command = Fakers.Businesses.UpdateBusinessPersonalDetailsCommandFaker.Generate();
        var invalidCommand = Fakers.Businesses.UpdateBusinessPersonalDetailsCommandFaker.Generate() with
        {
            PhoneNumbers = new List<PhoneNumber>(),
        };

        var result = await _sut.TestValidateAsync(invalidCommand);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumbers);
    }
}
