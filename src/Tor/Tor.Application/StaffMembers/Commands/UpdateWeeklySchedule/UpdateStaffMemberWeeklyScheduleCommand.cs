using FluentResults;
using MediatR;
using Tor.Domain.BusinessAggregate.ValueObjects;

namespace Tor.Application.StaffMembers.Commands.UpdateWeeklySchedule;

public record UpdateStaffMemberWeeklyScheduleCommand(
    Guid StaffMemberId,
    WeeklySchedule WeeklySchedule) : IRequest<Result>;