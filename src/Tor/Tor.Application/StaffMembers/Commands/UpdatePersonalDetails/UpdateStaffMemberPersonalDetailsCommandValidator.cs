using FluentValidation;
using Tor.Application.Common.Validators;

namespace Tor.Application.StaffMembers.Commands.UpdatePersonalDetails;

public sealed class UpdateStaffMemberPersonalDetailsCommandValidator : AbstractValidator<UpdateStaffMemberPersonalDetailsCommand>
{
    public UpdateStaffMemberPersonalDetailsCommandValidator()
    {
        RuleFor(x => x.StaffMemberId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Address)
            .SetValidator(new AddressValidator())
            .When(x => x.Address is not null);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .SetValidator(new PhoneNumberValidator());
    }
}
