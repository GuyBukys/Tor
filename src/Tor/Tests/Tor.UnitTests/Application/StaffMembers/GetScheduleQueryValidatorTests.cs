using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.StaffMembers.Queries.GetSchedule;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.StaffMembers;

public class GetScheduleQueryValidatorTests
{
    private readonly GetScheduleQueryValidator _sut;

    public GetScheduleQueryValidatorTests()
    {
        _sut = new GetScheduleQueryValidator();
    }

    [Fact]
    public async Task Validate_WhenValidCommand_ShouldReturnTrue()
    {
        var query = Fakers.StaffMembers.GetScheduleQueryFaker.Generate();

        var result = await _sut.TestValidateAsync(query);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Validate_WhenFromIsGreaterOrEqualThanUntil_ShouldReturnFalse(bool isEqual)
    {
        var query = Fakers.StaffMembers.GetScheduleQueryFaker.Generate();
        var invalidQuery = query with
        {
            From = isEqual ? query.Until : query.Until!.Value.AddDays(1),
        };

        var result = await _sut.TestValidateAsync(invalidQuery);

        result.ShouldHaveValidationErrorFor(x => x.From);
    }
}
