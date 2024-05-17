using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.Common.Validators;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Common.Validators;

public class ClientDetailsValidatorTests
{
    private readonly ClientDetailsValidator _sut;

    public ClientDetailsValidatorTests()
    {
        _sut = new ClientDetailsValidator();
    }

    [Fact]
    public async Task Validate_WhenValidEntity_ShouldReturnTrue()
    {
        var clientDetails = Fakers.Appointments.ClientDetailsFaker.Generate();

        var result = await _sut.TestValidateAsync(clientDetails);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var clientDetails = Fakers.Appointments.ClientDetailsFaker.Generate() with
        {
            Name = null!,
        };

        var result = await _sut.TestValidateAsync(clientDetails);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Validate_WhenStringsAboveMaximumLength_ShouldReturnFalse()
    {
        var clientDetails = Fakers.Appointments.ClientDetailsFaker.Generate() with
        {
            Name = new string('A', 201),
        };

        var result = await _sut.TestValidateAsync(clientDetails);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
