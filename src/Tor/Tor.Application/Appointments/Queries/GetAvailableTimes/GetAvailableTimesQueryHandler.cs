using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Appointments.Common;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.Appointments.Queries.GetAvailableTimes;

internal sealed class GetAvailableTimesQueryHandler : IRequestHandler<GetAvailableTimesQuery, Result<List<AvailableTimes>>>
{
    private readonly ITorDbContext _context;
    private readonly IAvailableTimesCalculator _availableTimesCalculator;

    public GetAvailableTimesQueryHandler(
        ITorDbContext context,
        IAvailableTimesCalculator availableTimesCalculator)
    {
        _context = context;
        _availableTimesCalculator = availableTimesCalculator;
    }

    public async Task<Result<List<AvailableTimes>>> Handle(GetAvailableTimesQuery request, CancellationToken cancellationToken)
    {
        Service? service = await _context.Services
            .AsNoTracking()
            .Include(x => x.StaffMember)
            .ThenInclude(x => x.Business)
            .FirstOrDefaultAsync(x => x.Id == request.ServiceId, cancellationToken);

        if (service is null)
        {
            return Result.Fail<List<AvailableTimes>>(
                new NotFoundError($"could not find service with id {request.ServiceId}"));
        }

        TimeSpan duration = TimeSpan.FromMinutes(service.Durations.First().ValueInMinutes);
        IEnumerable<DateOnly> dates = AppointmentUtils.GetDatesBetween(request.StartDate, request.EndDate);

        List<AvailableTimes> availableTimes = [];
        foreach (DateOnly date in dates)
        {
            List<TimeRange> timeRanges = await _availableTimesCalculator.CalculateAvailableTimes(
                date,
                duration,
                service.StaffMember.Business.Settings,
                service.StaffMember.WeeklySchedule,
                service.StaffMemberId,
                cancellationToken);

            if (timeRanges.Count > 0)
            {
                availableTimes.Add(new(date, timeRanges));
            }
        }

        return availableTimes;
    }
}
