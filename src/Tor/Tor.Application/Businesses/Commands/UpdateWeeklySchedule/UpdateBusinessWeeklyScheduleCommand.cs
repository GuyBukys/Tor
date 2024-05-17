using FluentResults;
using MediatR;
using Tor.Domain.BusinessAggregate.ValueObjects;

namespace Tor.Application.Businesses.Commands.UpdateWeeklySchedule;

public record UpdateBusinessWeeklyScheduleCommand(
    Guid BusinessId,
    WeeklySchedule? WeeklySchedule) : IRequest<Result>;