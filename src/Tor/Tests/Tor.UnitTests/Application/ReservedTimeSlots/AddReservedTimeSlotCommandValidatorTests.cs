using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.ReservedTimeSlots.Commands.Add;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.ReservedTimeSlots;

public class AddReservedTimeSlotCommandValidatorTests
{
    private readonly AddReservedTimeSlotCommandValidator _sut;

    public AddReservedTimeSlotCommandValidatorTests()
    {
        _sut = new AddReservedTimeSlotCommandValidator();
    }

    [Fact]
    public async Task Validate_WhenValidCommand_ShouldReturnTrue()
    {
        var command = Fakers.ReservedTimeSlots.AddReservedTimeSlotCommandFaker.Generate();

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var command = Fakers.ReservedTimeSlots.AddReservedTimeSlotCommandFaker.Generate() with
        {
            StaffMemberId = Guid.Empty,
            AtDate = default,
            TimeRange = null!,
        };

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.StaffMemberId);
        result.ShouldHaveValidationErrorFor(x => x.AtDate);
        result.ShouldHaveValidationErrorFor(x => x.TimeRange);
    }
}
