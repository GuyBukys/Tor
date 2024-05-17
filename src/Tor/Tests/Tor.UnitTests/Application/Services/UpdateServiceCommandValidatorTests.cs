using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.Services.Commands.UpdateService;
using Tor.Domain.BusinessAggregate.Enums;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Services;
public class UpdateServiceCommandValidatorTests
{
    private readonly UpdateServiceCommandValidator _sut;

    public UpdateServiceCommandValidatorTests()
    {
        _sut = new UpdateServiceCommandValidator();
    }

    [Fact]
    public async Task Validate_WhenValidCommand_ShouldReturnTrue()
    {
        var command = Fakers.Services.UpdateServiceCommandFaker.Generate();

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var command = Fakers.Services.UpdateServiceCommandFaker.Generate();
        var invalidcommand = command with
        {
            ServiceId = Guid.Empty,
            Name = string.Empty,
            Amount = null!,
            Durations = new List<Duration>(),
        };

        var result = await _sut.TestValidateAsync(invalidcommand);
        result.ShouldHaveValidationErrorFor(x => x.ServiceId);
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
        result.ShouldHaveValidationErrorFor(x => x.Durations);
    }

    [Fact]
    public async Task Validate_WhenDurationsNotValid_ShouldReturnFalse()
    {
        var command = Fakers.Services.UpdateServiceCommandFaker.Generate();
        var invalidcommand = command with
        {
            Durations = new List<Duration>()
            {
                new Duration(1, 60, DurationType.Work),
                new Duration(1, 60, DurationType.Gap),
            },
        };

        var result = await _sut.TestValidateAsync(invalidcommand);
        result.ShouldHaveValidationErrorFor(x => x.Durations);
    }
}
