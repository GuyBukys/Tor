using FluentResults;
using MediatR;

namespace Tor.Application.StaffMembers.Queries.GetSchedule;

public record GetScheduleQuery(
    Guid StaffMemberId,
    DateTimeOffset? From,
    DateTimeOffset? Until) : IRequest<Result<GetScheduleResult>>;
