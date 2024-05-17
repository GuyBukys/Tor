using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.Appointments.Common;

internal static class AppointmentUtils
{
    internal static IEnumerable<TimeRange> BuildSections(TimeRange timeRange, TimeSpan duration)
    {
        List<TimeRange> sections = [];
        TimeOnly current = timeRange.StartTime;

        while (current <= timeRange.EndTime)
        {
            TimeOnly next = current.Add(duration);
            TimeRange currentSection = new(current, next);

            bool isInsideWorkingHours =
                currentSection.StartTime >= timeRange.StartTime &&
                currentSection.EndTime >= timeRange.StartTime &&
                currentSection.StartTime <= timeRange.EndTime &&
                currentSection.EndTime <= timeRange.EndTime;
            if (isInsideWorkingHours)
            {
                sections.Add(currentSection);
            }

            if (next < current) // we jumped to the next day
            {
                break;
            }
            current = next;
        }

        return sections;
    }

    /// <summary>
    /// Returns the filtered sections from the time ranges to filter
    /// </summary>
    /// <param name="sections"></param>
    /// <param name="timeRangesToFilter"></param>
    /// <returns></returns>
    internal static IEnumerable<TimeRange> FilterOverlapTimeRanges(
        IEnumerable<TimeRange> sections,
        IEnumerable<TimeRange> timeRangesToFilter)
    {
        foreach (var currentSection in sections)
        {
            bool isOverlap = timeRangesToFilter.Any(x =>
                (currentSection.StartTime >= x.StartTime && currentSection.EndTime <= x.EndTime) || //inside the range
                (currentSection.StartTime < x.StartTime && currentSection.EndTime > x.StartTime) || // overlaps with start of range
                (currentSection.StartTime < x.EndTime && currentSection.EndTime > x.EndTime)); // overlaps with end of range

            if (!isOverlap)
            {
                yield return currentSection;
            }
        }
    }

    /// <summary>
    /// Returns a list of dates between start date and end date
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    internal static IEnumerable<DateOnly> GetDatesBetween(DateOnly startDate, DateOnly endDate)
    {
        for (DateOnly currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
        {
            yield return currentDate;
        }
    }
}
