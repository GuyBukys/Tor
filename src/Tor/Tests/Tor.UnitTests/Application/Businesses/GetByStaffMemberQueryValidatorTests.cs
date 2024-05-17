using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.Businesses.Queries.GetByStaffMember;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.StaffMembers;

public class GetByStaffMemberQueryValidatorTests
{
    private readonly GetByStaffMemberQueryValidator _sut;

    public GetByStaffMemberQueryValidatorTests()
    {
        _sut = new GetByStaffMemberQueryValidator();
    }

    [Fact]
    public async Task Validate_WhenValidQuery_ShouldReturnTrue()
    {
        var query = Fakers.Businesses.GetByStaffMemberQueryFaker.Generate();

        var result = await _sut.TestValidateAsync(query);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var query = Fakers.Businesses.GetByStaffMemberQueryFaker.Generate();
        var invalidQuery = query with
        {
            StaffMemberId = Guid.Empty,
        };

        var result = await _sut.TestValidateAsync(invalidQuery);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.StaffMemberId);
    }
}
