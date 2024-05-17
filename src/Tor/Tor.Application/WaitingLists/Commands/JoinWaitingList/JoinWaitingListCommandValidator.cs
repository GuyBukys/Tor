using FluentValidation;

namespace Tor.Application.WaitingLists.Commands.JoinWaitingList;

public sealed class JoinWaitingListCommandValidator : AbstractValidator<JoinWaitingListCommand>
{
    public JoinWaitingListCommandValidator()
    {
        RuleFor(x => x.StaffMemberId)
            .NotEmpty();

        RuleFor(x => x.ClientId)
            .NotEmpty();

        RuleFor(x => x.AtDate)
            .NotEmpty()
            .Must(x => x >= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("atDate must be a current or future date");
    }
}
