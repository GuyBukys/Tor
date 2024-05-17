using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.Appointments.Commands.CancelAppointment;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Appointments;
public class CancelAppointmentCommandValidatorTests
{
    private readonly CancelAppointmentCommandValidator _sut;

    public CancelAppointmentCommandValidatorTests()
    {
        _sut = new CancelAppointmentCommandValidator();
    }

    [Fact]
    public async Task Validate_WhenValidCommand_ShouldReturnTrue()
    {
        var command = Fakers.Appointments.CancelAppointmentCommandFaker.Generate();

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var command = Fakers.Appointments.CancelAppointmentCommandFaker.Generate();
        var invalidCommand = command with
        {
            AppointmentId = Guid.Empty,
        };

        var result = await _sut.TestValidateAsync(invalidCommand);

        result.ShouldHaveValidationErrorFor(x => x.AppointmentId);
    }
}
