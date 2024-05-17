using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.Businesses.Commands.UpdateSettings;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Businesses;

public class UpdateBusinessSettingsCommandValidatorTests
{
    private readonly UpdateBusinessSettingsCommandValidator _sut;

    public UpdateBusinessSettingsCommandValidatorTests()
    {
        _sut = new UpdateBusinessSettingsCommandValidator();
    }

    [Fact]
    public async Task Validate_WhenValidCommand_ShouldReturnTrue()
    {
        var command = Fakers.Businesses.UpdateBusinessSettingsCommandFaker.Generate();

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var command = Fakers.Businesses.UpdateBusinessSettingsCommandFaker.Generate();
        var invalidCommand = Fakers.Businesses.UpdateBusinessSettingsCommandFaker.Generate() with
        {
            BusinessId = Guid.Empty,
            BusinessSettings = null!,
        };

        var result = await _sut.TestValidateAsync(invalidCommand);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.BusinessId);
        result.ShouldHaveValidationErrorFor(x => x.BusinessSettings);
    }

    [Fact]
    public async Task Validate_WhenBusinessSettingsIsInvalid_ShouldReturnFalse()
    {
        var command = Fakers.Businesses.UpdateBusinessSettingsCommandFaker.Generate();
        var invalidCommand = Fakers.Businesses.UpdateBusinessSettingsCommandFaker.Generate() with
        {
            BusinessSettings = new(-1, -1, -1, -1, -1, -1),
        };

        var result = await _sut.TestValidateAsync(invalidCommand);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.BusinessSettings.BookingMinimumTimeInAdvanceInMinutes);
        result.ShouldHaveValidationErrorFor(x => x.BusinessSettings.BookingMaximumTimeInAdvanceInDays);
        result.ShouldHaveValidationErrorFor(x => x.BusinessSettings.CancelAppointmentMinimumTimeInMinutes);
        result.ShouldHaveValidationErrorFor(x => x.BusinessSettings.RescheduleAppointmentMinimumTimeInMinutes);
        result.ShouldHaveValidationErrorFor(x => x.BusinessSettings.MaximumAppointmentsForClient);
        result.ShouldHaveValidationErrorFor(x => x.BusinessSettings.AppointmentReminderTimeInHours);
    }

    [Fact]
    public async Task Validate_WhenBookingMinimumTimeIsGreaterOrEqualToMaximumTime_ShouldReturnFalse()
    {
        var command = Fakers.Businesses.UpdateBusinessSettingsCommandFaker.Generate();
        var invalidCommand = Fakers.Businesses.UpdateBusinessSettingsCommandFaker.Generate() with
        {
            BusinessSettings = command.BusinessSettings with
            {
                BookingMinimumTimeInAdvanceInMinutes = 1440 * 2,
                BookingMaximumTimeInAdvanceInDays = 1,
            }
        };

        var result = await _sut.TestValidateAsync(invalidCommand);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.BusinessSettings.BookingMaximumTimeInAdvanceInDays)
            .WithErrorMessage("business settings: booking minimum time in advance cannot be greater than booking maximum time in advance");
    }
}
