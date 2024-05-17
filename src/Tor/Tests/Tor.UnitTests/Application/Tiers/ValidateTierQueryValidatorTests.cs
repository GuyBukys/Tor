using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.Tiers.Queries.ValidateTier;

namespace Tor.UnitTests.Application.Tiers;

public class ValidateTierQueryValidatorTests
{
    private readonly ValidateTierQueryValidator _sut;

    public ValidateTierQueryValidatorTests()
    {
        _sut = new ValidateTierQueryValidator();
    }

    [Fact]
    public async Task Validate_WhenValidQuery_ShouldReturnTrue()
    {
        var query = new ValidateTierQuery(Guid.NewGuid(), "offering id");

        var result = await _sut.TestValidateAsync(query);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var query = new ValidateTierQuery(Guid.Empty, string.Empty);

        var result = await _sut.TestValidateAsync(query);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.StaffMemberId);
    }
}
