using Tor.Application.Services.Queries.GetServices;
using Tor.TestsInfra.CustomFakers;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace Tor.UnitTests.Application.Services;
public class GetServicesQueryValidatorTests
{
    private readonly GetServicesQueryValidator _sut;

    public GetServicesQueryValidatorTests()
    {
        _sut = new GetServicesQueryValidator();
    }

    [Fact]
    public async Task Validate_WhenValidCommand_ShouldReturnTrue()
    {
        var query = Fakers.Services.GetServicesQueryFaker.Generate();

        var result = await _sut.TestValidateAsync(query);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var query = Fakers.Services.GetServicesQueryFaker.Generate();
        var invalidQuery = query with
        {
            StaffMemberId = Guid.Empty,
        };

        var result = await _sut.TestValidateAsync(invalidQuery);

        result.ShouldHaveValidationErrorFor(x => x.StaffMemberId);
    }
}
