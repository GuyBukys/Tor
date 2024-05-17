using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.WaitingLists.Commands.JoinWaitingList;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.WaitingLists;

public class JoinWaitingListCommandValidatorTests
{
    private readonly JoinWaitingListCommandValidator _sut;

    public JoinWaitingListCommandValidatorTests()
    {
        _sut = new JoinWaitingListCommandValidator();
    }

    [Fact]
    public async Task Validate_WhenValidCommand_ShouldReturnTrue()
    {
        var command = Fakers.WaitingLists.JoinWaitingListCommandFaker.Generate();

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreEmpty_ShouldReturnFalse()
    {
        var command = Fakers.WaitingLists.JoinWaitingListCommandFaker.Generate() with
        {
            StaffMemberId = Guid.Empty,
            ClientId = Guid.Empty,
            AtDate = default!,
        };

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.StaffMemberId);
        result.ShouldHaveValidationErrorFor(x => x.ClientId);
        result.ShouldHaveValidationErrorFor(x => x.AtDate);
    }

    [Fact]
    public async Task Validate_WhenAtDateIsNotAFutureDate_ShouldReturnFalse()
    {
        var command = Fakers.WaitingLists.JoinWaitingListCommandFaker.Generate() with
        {
            AtDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
        };

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.AtDate)
            .WithErrorMessage("atDate must be a current or future date");
    }
}
