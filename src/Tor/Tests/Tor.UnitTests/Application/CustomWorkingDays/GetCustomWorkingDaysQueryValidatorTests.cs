using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.CustomWorkingDays.Queries.GetCustomWorkingDays;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.CustomWorkingDays;
public class GetCustomWorkingDaysQueryValidatorTests
{
    private readonly GetCustomWorkingDaysQueryValidator _sut;

    public GetCustomWorkingDaysQueryValidatorTests()
    {
        _sut = new GetCustomWorkingDaysQueryValidator();
    }

    [Fact]
    public async Task Validate_WhenValidCommand_ShouldReturnTrue()
    {
        var query = Fakers.StaffMembers.GetCustomWorkingDaysQueryFaker.Generate();

        var result = await _sut.TestValidateAsync(query);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var query = Fakers.StaffMembers.GetCustomWorkingDaysQueryFaker.Generate();
        var invalidQuery = query with
        {
            StaffMemberId = Guid.Empty,
        };

        var result = await _sut.TestValidateAsync(invalidQuery);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WhenFromIsGreaterThanUntil_ShouldReturnFalse()
    {
        var query = Fakers.StaffMembers.GetCustomWorkingDaysQueryFaker.Generate();
        var invalidQuery = query with
        {
            StaffMemberId = Guid.NewGuid(),
            From = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            Until = DateOnly.FromDateTime(DateTime.UtcNow),
        };

        var result = await _sut.TestValidateAsync(invalidQuery);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.From);
    }
}
