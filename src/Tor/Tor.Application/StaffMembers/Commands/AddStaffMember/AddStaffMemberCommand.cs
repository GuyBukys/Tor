using FluentResults;
using MediatR;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.StaffMembers.Commands.AddStaffMember;

public record AddStaffMemberCommand(
    Guid BusinessId,
    Guid UserId,
    string Name,
    string Email,
    WeeklySchedule WeeklySchedule,
    PhoneNumber PhoneNumber,
    List<AddStaffMemberServiceCommand> Services,
    Image? ProfileImage) : IRequest<Result<StaffMember>>;

public record AddStaffMemberServiceCommand(
    string Name,
    AmountDetails Amount,
    List<Duration> Durations);