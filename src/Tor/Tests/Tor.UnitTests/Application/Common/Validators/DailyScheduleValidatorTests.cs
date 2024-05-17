using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.Common.Validators;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Common.Validators;

public class DailyScheduleValidatorTests
{
    private readonly DailyScheduleValidator _sut;

    public DailyScheduleValidatorTests()
    {
        _sut = new DailyScheduleValidator();
    }

    [Fact]
    public async Task Validate_WhenValiddailySchedule_ShouldReturnTrue()
    {
        var dailySchedule = Fakers.Common.DailyScheduleFaker.Generate();

        var result = await _sut.TestValidateAsync(dailySchedule);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenTimeRangeIsNull_ShouldReturnFalse()
    {
        var dailySchedule = Fakers.Common.DailyScheduleFaker.Generate();
        dailySchedule.TimeRange = null!;

        var result = await _sut.TestValidateAsync(dailySchedule);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.TimeRange);
    }

    [Fact]
    public async Task Validate_WhenRecurringBreaksIsNull_ShouldReturnFalse()
    {
        var dailySchedule = Fakers.Common.DailyScheduleFaker.Generate();
        dailySchedule.RecurringBreaks = null!;

        var result = await _sut.TestValidateAsync(dailySchedule);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.RecurringBreaks);
    }

    [Fact]
    public async Task Validate_WhenTimeRangeIsNotValid_ShouldReturnFalse()
    {
        var dailySchedule = Fakers.Common.DailyScheduleFaker.Generate();
        dailySchedule.TimeRange = new(new TimeOnly(8, 0), new TimeOnly(7, 0));

        var result = await _sut.TestValidateAsync(dailySchedule);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.TimeRange!.StartTime);
    }

    [Fact]
    public async Task Validate_WhenSomeRecurringBreaksAreNull_ShouldReturnFalse()
    {
        var dailySchedule = Fakers.Common.DailyScheduleFaker.Generate();
        dailySchedule.RecurringBreaks = [null!];

        var result = await _sut.TestValidateAsync(dailySchedule);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.RecurringBreaks);
    }

    [Fact]
    public async Task Validate_WhenFieldsAreNullAndIsNotWorkingDay_ShouldReturnTrue()
    {
        var dailySchedule = new DailySchedule()
        {
            IsWorkingDay = false,
            TimeRange = null!,
            RecurringBreaks = null!,
        };

        var result = await _sut.TestValidateAsync(dailySchedule);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenSomeRecurringBreaksAreInvalid_ShouldReturnFalse()
    {
        var dailySchedule = Fakers.Common.DailyScheduleFaker.Generate();
        dailySchedule.RecurringBreaks =
        [
            new(1, new(new TimeOnly(8, 0), new TimeOnly(7, 0)))
        ];

        var result = await _sut.TestValidateAsync(dailySchedule);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor("RecurringBreaks[0].TimeRange.StartTime");
    }

    [Fact]
    public async Task Validate_WhenSomeRecurringBreaksAreNotInDailyScheduleTimeRange_ShouldReturnFalse()
    {
        var dailySchedule = Fakers.Common.DailyScheduleFaker.Generate();
        dailySchedule.TimeRange = new(new TimeOnly(8, 0), new TimeOnly(16, 0));
        dailySchedule.RecurringBreaks =
        [
            new(1, new(new TimeOnly(17, 0), new TimeOnly(18, 0))),
        ];

        var result = await _sut.TestValidateAsync(dailySchedule);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == "daily schedule: some recurring breaks are not in daily schedule time range");
    }
}
