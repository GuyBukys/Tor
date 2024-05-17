using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.Clients.Commands.Create;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Clients;

public class CreateClientCommandValidatorTests
{
    private readonly CreateClientCommandValidator _sut;

    public CreateClientCommandValidatorTests()
    {
        _sut = new CreateClientCommandValidator();
    }

    [Fact]
    public async Task Validate_WhenValidCommand_ShouldReturnTrue()
    {
        var command = Fakers.Clients.CreateClientCommandFaker.Generate();

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var command = Fakers.Clients.CreateClientCommandFaker.Generate() with
        {
            UserId = Guid.Empty,
            Name = null!,
            Email = null!,
            PhoneNumber = null!,
        };

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.UserId);
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Email);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData("")]
    [InlineData("test@")]
    [InlineData("not a valid email")]
    [InlineData("test.com")]
    public async Task Validate_WhenEmailIsInvalid_ShouldReturnFalse(string email)
    {
        var command = Fakers.Clients.CreateClientCommandFaker.Generate() with
        {
            Email = email,
        };

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
}
