using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.ReservedTimeSlots.Commands.Update;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.ReservedTimeSlots;

public class UpdateReservedTimeSlotCommandValidatorTests
{
    private readonly UpdateReservedTimeSlotCommandValidator _sut;

    public UpdateReservedTimeSlotCommandValidatorTests()
    {
        _sut = new UpdateReservedTimeSlotCommandValidator();
    }

    [Fact]
    public async Task Validate_WhenValidCommand_ShouldReturnTrue()
    {
        var command = Fakers.ReservedTimeSlots.UpdateReservedTimeSlotCommandFaker.Generate();

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var command = Fakers.ReservedTimeSlots.UpdateReservedTimeSlotCommandFaker.Generate() with
        {
            Id = Guid.Empty,
            AtDate = default,
            TimeRange = null!,
        };

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Id);
        result.ShouldHaveValidationErrorFor(x => x.AtDate);
        result.ShouldHaveValidationErrorFor(x => x.TimeRange);
    }
}
