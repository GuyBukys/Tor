using FluentResults;
using MediatR;

namespace Tor.Application.MessageBlasts.Commands.AppointmentReminder;

public record AppointmentReminderCommand() : IRequest<Result>;
