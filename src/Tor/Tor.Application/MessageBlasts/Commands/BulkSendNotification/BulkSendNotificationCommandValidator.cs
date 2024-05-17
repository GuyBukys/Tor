using FluentValidation;

namespace Tor.Application.MessageBlasts.Commands.BulkSendNotification;

public sealed class BulkSendNotificationCommandValidator : AbstractValidator<BulkSendNotificationCommand>
{
    public BulkSendNotificationCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Message)
            .NotEmpty()
            .MaximumLength(10000);
    }
}
