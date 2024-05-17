using FluentResults;
using MediatR;

namespace Tor.Application.Appointments.Queries.GetAvailableTimes;

public record GetAvailableTimesQuery(
    Guid ServiceId,
    DateOnly StartDate,
    DateOnly EndDate) : IRequest<Result<List<AvailableTimes>>>;
