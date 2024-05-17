using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.Businesses.Commands.UpdateImages;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Businesses;

public class UpdateBusinessImagesCommandValidatorTests
{
    private readonly UpdateBusinessImagesCommandValidator _sut;

    public UpdateBusinessImagesCommandValidatorTests()
    {
        _sut = new UpdateBusinessImagesCommandValidator();
    }

    [Fact]
    public async Task Validate_WhenValidCommand_ShouldReturnTrue()
    {
        var command = Fakers.Businesses.UpdateBusinessImagesCommandFaker.Generate();

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var command = Fakers.Businesses.UpdateBusinessImagesCommandFaker.Generate() with
        {
            BusinessId = Guid.Empty,
        };

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.BusinessId);
    }

    [Fact]
    public async Task Validate_WhenOneOfPortfolioIsNull_ShouldReturnFalse()
    {
        var command = Fakers.Businesses.UpdateBusinessImagesCommandFaker.Generate() with
        {
            Portfolio = [null!,]
        };

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor("Portfolio[0]");
    }
}
