using FluentResults;
using MediatR;

namespace Tor.Application.MessageBlasts.Commands.ScheduleAppointmentReminder;

public record ScheduleAppointmentReminderCommand : IRequest<Result>;
