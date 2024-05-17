using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.Appointments.Commands.ScheduleAppointment;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Appointments;
public class ScheduleAppointmentCommandValidatorTests
{
    private readonly ScheduleAppointmentCommandValidator _sut;

    public ScheduleAppointmentCommandValidatorTests()
    {
        _sut = new ScheduleAppointmentCommandValidator();
    }

    [Fact]
    public async Task Validate_WhenValidCommand_ShouldReturnTrue()
    {
        var command = Fakers.Appointments.ScheduleAppointmentCommandFaker.Generate();

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var command = Fakers.Appointments.ScheduleAppointmentCommandFaker.Generate();
        var invalidCommand = command with
        {
            StaffMemberId = Guid.Empty,
            ScheduledFor = default!,
            ClientDetails = null!,
            ServiceDetails = null!,
        };

        var result = await _sut.TestValidateAsync(invalidCommand);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.StaffMemberId);
        result.ShouldHaveValidationErrorFor(x => x.ScheduledFor);
        result.ShouldHaveValidationErrorFor(x => x.ClientDetails);
        result.ShouldHaveValidationErrorFor(x => x.ServiceDetails);
    }

    [Fact]
    public async Task Validate_WhenAppointmentTypeInvalid_ShouldReturnFalse()
    {
        var command = Fakers.Appointments.ScheduleAppointmentCommandFaker.Generate();
        var invalidCommand = command with
        {
            Type = 0,
        };

        var result = await _sut.TestValidateAsync(invalidCommand);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Type);
    }
}
