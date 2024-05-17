using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.Businesses.Queries.GetByInvitation;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.StaffMembers;

public class GetByInvitationQueryValidatorTests
{
    private readonly GetByInvitationQueryValidator _sut;

    public GetByInvitationQueryValidatorTests()
    {
        _sut = new GetByInvitationQueryValidator();
    }

    [Fact]
    public async Task Validate_WhenValidQuery_ShouldReturnTrue()
    {
        var query = Fakers.Businesses.GetByInvitationQueryFaker.Generate();

        var result = await _sut.TestValidateAsync(query);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var query = Fakers.Businesses.GetByInvitationQueryFaker.Generate();
        var invalidQuery = query with
        {
            InvitationId = null!,
        };

        var result = await _sut.TestValidateAsync(invalidQuery);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.InvitationId);
    }
}
