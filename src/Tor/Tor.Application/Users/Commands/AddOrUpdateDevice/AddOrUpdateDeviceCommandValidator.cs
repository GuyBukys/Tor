using FluentValidation;

namespace Tor.Application.Users.Commands.AddOrUpdateDevice;

public sealed class AddOrUpdateDeviceCommandValidator : AbstractValidator<AddOrUpdateDeviceCommand>
{
    public AddOrUpdateDeviceCommandValidator()
    {
        RuleFor(x => x.UserToken)
            .NotEmpty();

        RuleFor(x => x.DeviceToken)
            .NotEmpty();
    }
}
