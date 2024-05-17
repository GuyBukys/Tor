using FluentValidation;
using Tor.Application.Businesses.Commands.Create;
using Tor.Application.Common.Validators;

namespace Tor.Application.Users.Commands.Create;

public sealed class CreateBusinessCommandValidator : AbstractValidator<CreateBusinessCommand>
{
    public CreateBusinessCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.CategoryIds)
            .NotEmpty();

        RuleFor(x => x.Cover)
            .SetValidator(new ImageValidator())
            .When(x => x.Cover is not null);

        RuleFor(x => x.CategoryIds)
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("categories: category ids are not distinct");

        RuleFor(x => x.BusinessOwner)
            .NotNull()
            .SetValidator(new CreateBusinessOwnerValidator());

        RuleFor(x => x.WeeklySchedule)
            .NotEmpty()
            .SetValidator(new WeeklyScheduleValidator());

        RuleFor(x => x.Address)
            .SetValidator(new AddressValidator())
            .When(x => x.Address is not null);

        RuleFor(x => x.PhoneNumbers)
            .NotEmpty()
            .ForEach(x => x.SetValidator(new PhoneNumberValidator()));
    }
}

class CreateBusinessOwnerValidator : InlineValidator<BusinessOwnerCommand>
{
    public CreateBusinessOwnerValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.PhoneNumber)
            .SetValidator(new PhoneNumberValidator());

        RuleFor(x => x.Services)
            .NotEmpty()
            .ForEach(x => x.SetValidator(new CreateServiceCommandValidator()));

        RuleFor(x => x.ProfileImage)
            .SetValidator(new ImageValidator())
            .When(x => x.ProfileImage is not null);
    }
}

class CreateServiceCommandValidator : InlineValidator<ServiceCommand>
{
    public CreateServiceCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Amount)
            .NotNull()
            .SetValidator(new AmountDetailsValidator());

        RuleFor(x => x.Durations)
            .NotEmpty()
            .ForEach(x => x.SetValidator(new DurationValidator()))
            .Must(x => x.DistinctBy(x => x.Order).Count() == x.Count());
    }
}