using FluentValidation;
using Tor.Application.Common.Validators;

namespace Tor.Application.StaffMembers.Commands.AddStaffMember;

public sealed class AddStaffMemberCommandValidator : AbstractValidator<AddStaffMemberCommand>
{
    public AddStaffMemberCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .SetValidator(new PhoneNumberValidator());

        RuleFor(x => x.Services)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .ForEach(x => x.NotEmpty())
            .ForEach(x => x.SetValidator(new ServiceCommandValidator()));

        RuleFor(x => x.ProfileImage)
            .SetValidator(new ImageValidator())
            .When(x => x.ProfileImage is not null);
    }
}

public sealed class ServiceCommandValidator : InlineValidator<AddStaffMemberServiceCommand>
{
    public ServiceCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Amount)
            .NotNull()
            .SetValidator(new AmountDetailsValidator());

        RuleFor(x => x.Durations)
            .NotEmpty()
            .ForEach(x => x.SetValidator(new DurationValidator()));
    }
}
