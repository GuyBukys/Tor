using FluentValidation;
using Tor.Application.Common.Validators;

namespace Tor.Application.StaffMembers.Commands.UpdateStaffMemberAddress;

public sealed class UpdateStaffMemberAddressCommandValidator : AbstractValidator<UpdateStaffMemberAddressCommand>
{
    public UpdateStaffMemberAddressCommandValidator()
    {
        RuleFor(x => x.StaffMemberId)
            .NotEmpty();

        RuleFor(x => x.Address)
            .NotEmpty()
            .SetValidator(new AddressValidator());
    }
}
