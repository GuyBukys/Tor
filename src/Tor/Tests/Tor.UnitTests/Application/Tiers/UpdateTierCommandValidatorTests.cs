using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.Tiers.Commands.UpdateTier;
using Tor.Domain.TierAggregate.Enums;

namespace Tor.UnitTests.Application.Tiers;

public class UpdateTierCommandValidatorTests
{
    private readonly UpdateTierCommandValidator _sut;

    public UpdateTierCommandValidatorTests()
    {
        _sut = new UpdateTierCommandValidator();
    }

    [Fact]
    public async Task Validate_WhenValidCommand_ShouldReturnTrue()
    {
        var command = new UpdateTierCommand(Guid.NewGuid(), TierType.Premium);

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var command = new UpdateTierCommand(Guid.Empty, (TierType)0);

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.BusinessId);
        result.ShouldHaveValidationErrorFor(x => x.TierType);
    }
}
