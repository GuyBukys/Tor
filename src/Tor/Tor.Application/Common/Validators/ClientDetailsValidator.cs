using FluentValidation;
using Tor.Domain.AppointmentAggregate.ValueObjects;

namespace Tor.Application.Common.Validators;

public sealed class ClientDetailsValidator : AbstractValidator<ClientDetails>
{
    public ClientDetailsValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}
