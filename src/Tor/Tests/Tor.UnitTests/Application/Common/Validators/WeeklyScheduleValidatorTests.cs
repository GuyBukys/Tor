using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.Common.Validators;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Common.Validators;

public class WeeklyScheduleValidatorTests
{
    private readonly WeeklyScheduleValidator _sut;

    public WeeklyScheduleValidatorTests()
    {
        _sut = new WeeklyScheduleValidator();
    }

    [Fact]
    public async Task Validate_WhenValidCommand_ShouldReturnTrue()
    {
        var command = Fakers.Common.WeeklyScheduleFaker.Generate();

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var invalidCommand = new WeeklySchedule(
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        var result = await _sut.TestValidateAsync(invalidCommand);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Sunday);
        result.ShouldHaveValidationErrorFor(x => x.Monday);
        result.ShouldHaveValidationErrorFor(x => x.Tuesday);
        result.ShouldHaveValidationErrorFor(x => x.Wednesday);
        result.ShouldHaveValidationErrorFor(x => x.Thursday);
        result.ShouldHaveValidationErrorFor(x => x.Friday);
        result.ShouldHaveValidationErrorFor(x => x.Saturday);
    }
}
