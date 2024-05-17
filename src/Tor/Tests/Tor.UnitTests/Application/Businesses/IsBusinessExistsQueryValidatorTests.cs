using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.Businesses.Queries.IsBusinessExists;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Businesses;

public class IsBusinessExistsQueryValidatorTests : UnitTestBase
{
    private readonly IsBusinessExistsQueryValidator _sut;

    public IsBusinessExistsQueryValidatorTests()
    {
        _sut = new IsBusinessExistsQueryValidator();
    }

    [Fact]
    public async Task Validate_WhenValidQuery_ShouldReturnTrue()
    {
        var query = Fakers.Businesses.IsBusinessExistsQueryFaker.Generate();

        var result = await _sut.TestValidateAsync(query);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse(string email)
    {
        var query = Fakers.Businesses.IsBusinessExistsQueryFaker.Generate();
        var invalidQuery = query with
        {
            Email = email,
        };

        var result = await _sut.TestValidateAsync(invalidQuery);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task Validate_WhenEmailIsInvalid_ShouldReturnFalse()
    {
        var query = Fakers.Businesses.IsBusinessExistsQueryFaker.Generate();
        var invalidQuery = query with
        {
            Email = "not a valid email",
        };

        var result = await _sut.TestValidateAsync(invalidQuery);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
}
