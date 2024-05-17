using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.Businesses.Commands.UpdateAddress;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Businesses;

public class UpdateBusinessAddressCommandValidatorTests
{
    private readonly UpdateBusinessAddressCommandValidator _sut;

    public UpdateBusinessAddressCommandValidatorTests()
    {
        _sut = new UpdateBusinessAddressCommandValidator();
    }

    [Fact]
    public async Task Validate_WhenValidCommand_ShouldReturnTrue()
    {
        var command = Fakers.Businesses.UpdateBusinessAddressCommandFaker.Generate();

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var command = new UpdateBusinessAddressCommand(Guid.Empty, null!);

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.BusinessId);
        result.ShouldHaveValidationErrorFor(x => x.Address);
    }
}
