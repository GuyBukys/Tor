using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.Appointments.Common;

public interface IAvailableTimesCalculator
{
    Task<List<TimeRange>> CalculateAvailableTimes(
        DateOnly atDate,
        TimeSpan duration,
        BusinessSettings settings,
        WeeklySchedule weeklySchedule,
        Guid staffMemberId,
        CancellationToken cancellationToken = default);
}
