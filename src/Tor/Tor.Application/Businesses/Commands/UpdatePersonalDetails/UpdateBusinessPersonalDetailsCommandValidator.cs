using FluentValidation;
using Tor.Application.Common.Validators;

namespace Tor.Application.Businesses.Commands.UpdatePersonalDetails;

public sealed class UpdateBusinessPersonalDetailsCommandValidator : AbstractValidator<UpdateBusinessPersonalDetailsCommand>
{
    public UpdateBusinessPersonalDetailsCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.PhoneNumbers)
            .NotEmpty()
            .ForEach(x => x.SetValidator(new PhoneNumberValidator()));
    }
}
