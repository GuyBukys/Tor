using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.Businesses.Queries.GetById;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.StaffMembers;

public class GetByIdQueryValidatorTests
{
    private readonly GetByIdQueryValidator _sut;

    public GetByIdQueryValidatorTests()
    {
        _sut = new GetByIdQueryValidator();
    }

    [Fact]
    public async Task Validate_WhenValidQuery_ShouldReturnTrue()
    {
        var query = Fakers.Businesses.GetByIdQueryFaker.Generate();

        var result = await _sut.TestValidateAsync(query);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var query = Fakers.Businesses.GetByIdQueryFaker.Generate();
        var invalidQuery = query with
        {
            Id = Guid.Empty,
        };

        var result = await _sut.TestValidateAsync(invalidQuery);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
