using FluentResults;
using MediatR;
using Tor.Domain.BusinessAggregate.ValueObjects;

namespace Tor.Application.CustomWorkingDays.Queries.GetCustomWorkingDays;

public record GetCustomWorkingDaysQuery(
    Guid StaffMemberId,
    DateOnly? From,
    DateOnly? Until) : IRequest<Result<List<CustomWorkingDay>>>;
