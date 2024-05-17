using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.Users.Commands.Create;
using Tor.Domain.UserAggregate.Enum;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Businesses;

public class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _sut;

    public CreateUserCommandValidatorTests()
    {
        _sut = new CreateUserCommandValidator();
    }

    [Fact]
    public async Task Validate_WhenValidCommand_ShouldReturnTrue()
    {
        var command = Fakers.Users.CreateUserCommandFaker.Generate();

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var command = Fakers.Users.CreateUserCommandFaker.Generate();
        var invalidCommand = command with
        {
            PhoneNumber = null!,
            UserToken = string.Empty,
        };

        var result = await _sut.TestValidateAsync(invalidCommand);

        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
        result.ShouldHaveValidationErrorFor(x => x.UserToken);
    }

    [Theory]
    [InlineData(((AppType)0))]
    public async Task Validate_WhenAppTypeIsInvalid_ShouldReturnFalse(AppType type)
    {
        var command = Fakers.Users.CreateUserCommandFaker.Generate();
        var invalidCommand = command with
        {
            AppType = type,
        };

        var result = await _sut.TestValidateAsync(invalidCommand);

        result.ShouldHaveValidationErrorFor(x => x.AppType);
    }
}
