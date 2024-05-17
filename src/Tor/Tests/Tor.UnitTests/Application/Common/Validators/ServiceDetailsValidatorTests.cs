using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.Common.Validators;
using Tor.Domain.AppointmentAggregate.ValueObjects;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Common.Validators;

public class ServiceDetailsValidatorTests
{
    private readonly ServiceDetailsValidator _sut;

    public ServiceDetailsValidatorTests()
    {
        _sut = new ServiceDetailsValidator();
    }

    [Fact]
    public async Task Validate_WhenValidEntity_ShouldReturnTrue()
    {
        var serviceDetails = Fakers.Appointments.ServiceDetailsFaker.Generate();

        var result = await _sut.TestValidateAsync(serviceDetails);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var serviceDetails = new ServiceDetails
        {
            Name = null!,
            Amount = null!,
            Durations = null!,
        };

        var result = await _sut.TestValidateAsync(serviceDetails);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
        result.ShouldHaveValidationErrorFor(x => x.Durations);
    }

    [Fact]
    public async Task Validate_WhenOneOfDurationsIsNull_ShouldReturnFalse()
    {
        var serviceDetails = Fakers.Appointments.ServiceDetailsFaker.Generate();
        serviceDetails.Durations.Add(null!);

        var result = await _sut.TestValidateAsync(serviceDetails);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Durations);
    }

    [Fact]
    public async Task Validate_WhenStringsAboveMaximumLength_ShouldReturnFalse()
    {
        var serviceDetails = Fakers.Appointments.ServiceDetailsFaker.Generate();
        serviceDetails.Name = new string('A', 201);
        serviceDetails.Description = new string('A', 1001);

        var result = await _sut.TestValidateAsync(serviceDetails);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
