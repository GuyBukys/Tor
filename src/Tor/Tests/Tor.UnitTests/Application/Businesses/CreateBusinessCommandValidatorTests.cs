using FluentAssertions;
using FluentValidation.TestHelper;
using Tor.Application.Businesses.Commands.Create;
using Tor.Application.Users.Commands.Create;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.Common.ValueObjects;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Businesses;

public class CreateBusinessCommandValidatorTests
{
    private readonly CreateBusinessCommandValidator _sut;

    public CreateBusinessCommandValidatorTests()
    {
        _sut = new CreateBusinessCommandValidator();
    }

    [Fact]
    public async Task Validate_WhenValidCommand_ShouldReturnTrue()
    {
        var command = Fakers.Businesses.CreateBusinessCommandFaker.Generate();

        var result = await _sut.TestValidateAsync(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var command = Fakers.Businesses.CreateBusinessCommandFaker.Generate();
        var invalidCommand = command with
        {
            Name = string.Empty,
            Description = string.Empty,
            Email = string.Empty,
            CategoryIds = new List<Guid>(),
            WeeklySchedule = null!,
            PhoneNumbers = new List<PhoneNumber>(),
            BusinessOwner = command.BusinessOwner with
            {
                UserId = Guid.Empty,
                Name = string.Empty,
                Email = string.Empty,
                Services = null!,
            },
        };

        var result = await _sut.TestValidateAsync(invalidCommand);

        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Description);
        result.ShouldHaveValidationErrorFor(x => x.Email);
        result.ShouldHaveValidationErrorFor(x => x.CategoryIds);
        result.ShouldHaveValidationErrorFor(x => x.BusinessOwner.UserId);
        result.ShouldHaveValidationErrorFor(x => x.BusinessOwner.Name);
        result.ShouldHaveValidationErrorFor(x => x.BusinessOwner.Email);
        result.ShouldHaveValidationErrorFor(x => x.WeeklySchedule);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumbers);
        result.ShouldHaveValidationErrorFor(x => x.BusinessOwner.Services);
    }

    [Fact]
    public async Task Validate_WhenBusinessOwnerIsNull_ShouldReturnFalse()
    {
        var command = Fakers.Businesses.CreateBusinessCommandFaker.Generate();
        var invalidCommand = command with
        {
            BusinessOwner = null!,
        };

        var result = await _sut.TestValidateAsync(invalidCommand);

        result.ShouldHaveValidationErrorFor(x => x.BusinessOwner);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid prefix")]
    [InlineData("+897123")]
    public async Task Validate_WhenPhoneNumberPrefixIsInvalid_ShouldReturnFalse(string prefix)
    {
        var command = Fakers.Businesses.CreateBusinessCommandFaker.Generate();
        var invalidCommand = command with
        {
            PhoneNumbers = new List<PhoneNumber>() { new(prefix, "522420554") },
        };

        var result = await _sut.TestValidateAsync(invalidCommand);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor("PhoneNumbers[0].Prefix");
    }

    [Fact]
    public async Task Validate_WhenCategoryIdsAreNotDistinct_ShouldReturnFalse()
    {
        Guid categoryId = Guid.NewGuid();
        var command = Fakers.Businesses.CreateBusinessCommandFaker.Generate();
        var invalidCommand = command with
        {
            CategoryIds = new List<Guid>() { categoryId, categoryId },
        };


        var result = await _sut.TestValidateAsync(invalidCommand);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.CategoryIds);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid number")]
    [InlineData("+1234!!@#")]
    public async Task Validate_WhenPhoneNumberNumberIsInvalid_ShouldReturnFalse(string number)
    {
        var command = Fakers.Businesses.CreateBusinessCommandFaker.Generate();
        var invalidCommand = command with
        {
            PhoneNumbers = new List<PhoneNumber>() { new("+972", number) },
        };

        var result = await _sut.TestValidateAsync(invalidCommand);

        result.ShouldHaveValidationErrorFor("PhoneNumbers[0].Number");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid prefix")]
    [InlineData("+897123")]
    public async Task Validate_WhenBusinessOwnerPhoneNumberPrefixIsInvalid_ShouldReturnFalse(string prefix)
    {
        var command = Fakers.Businesses.CreateBusinessCommandFaker.Generate();
        var invalidCommand = command with
        {
            BusinessOwner = command.BusinessOwner with
            {
                PhoneNumber = new PhoneNumber(prefix, "522420554"),
            }
        };

        var result = await _sut.TestValidateAsync(invalidCommand);

        result.ShouldHaveValidationErrorFor(x => x.BusinessOwner.PhoneNumber.Prefix);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid number")]
    [InlineData("+1234!!@#")]
    public async Task Validate_WhenBusinessOwnerPhoneNumberNumberIsInvalid_ShouldReturnFalse(string number)
    {
        var command = Fakers.Businesses.CreateBusinessCommandFaker.Generate();
        var invalidCommand = command with
        {
            BusinessOwner = command.BusinessOwner with
            {
                PhoneNumber = new PhoneNumber("+972", number),
            }
        };

        var result = await _sut.TestValidateAsync(invalidCommand);

        result.ShouldHaveValidationErrorFor(x => x.BusinessOwner.PhoneNumber.Number);
    }

    [Fact]
    public async Task Validate_WhenBusinessOwnerServiceMandatoryFieldsAreNullOrEmpty_ShouldReturnFalse()
    {
        var command = Fakers.Businesses.CreateBusinessCommandFaker.Generate();
        var validService = command.BusinessOwner.Services.First();
        var invalidCommand = command with
        {
            BusinessOwner = command.BusinessOwner with
            {
                Services = new List<ServiceCommand>()
                {
                    validService with
                    {
                        Name = string.Empty,
                        Amount = null!,
                        Durations = new List<Duration>(),
                    }
                }
            }
        };

        var result = await _sut.TestValidateAsync(invalidCommand);

        result.ShouldHaveValidationErrorFor("BusinessOwner.Services[0].Name");
        result.ShouldHaveValidationErrorFor("BusinessOwner.Services[0].Amount");
        result.ShouldHaveValidationErrorFor("BusinessOwner.Services[0].Durations");
    }

    [Theory]
    [InlineData(-1)]
    public async Task Validate_WhenBusinessOwnerServiceAmountIsInvalid_ShouldReturnFalse(double amount)
    {
        var command = Fakers.Businesses.CreateBusinessCommandFaker.Generate();
        var validService = command.BusinessOwner.Services.First();
        var invalidCommand = command with
        {
            BusinessOwner = command.BusinessOwner with
            {
                Services = new List<ServiceCommand>()
                {
                    validService with
                    {
                        Amount = new AmountDetails((decimal)amount, "ILS"),
                    }
                }
            }
        };

        var result = await _sut.TestValidateAsync(invalidCommand);

        result.ShouldHaveValidationErrorFor("BusinessOwner.Services[0].Amount.Amount");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("USD")]
    [InlineData("not a valid currency")]
    public async Task Validate_WhenBusinessOwnerServiceAmountCurrencyIsInvalid_ShouldReturnFalse(string currency)
    {
        var command = Fakers.Businesses.CreateBusinessCommandFaker.Generate();
        var validService = command.BusinessOwner.Services.First();
        var invalidCommand = command with
        {
            BusinessOwner = command.BusinessOwner with
            {
                Services = new List<ServiceCommand>()
                {
                    validService with
                    {
                        Amount = new AmountDetails(100, currency),
                    }
                }
            }
        };

        var result = await _sut.TestValidateAsync(invalidCommand);

        result.ShouldHaveValidationErrorFor("BusinessOwner.Services[0].Amount.Currency");
    }
}
