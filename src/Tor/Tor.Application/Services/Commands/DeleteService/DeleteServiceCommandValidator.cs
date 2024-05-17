using FluentValidation;

namespace Tor.Application.Services.Commands.DeleteService;

public sealed class DeleteServiceCommandValidator : AbstractValidator<DeleteServiceCommand>
{
    public DeleteServiceCommandValidator()
    {
        RuleFor(x => x.ServiceId)
            .NotEmpty();
    }
}
