using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tor.Application.Abstractions;
using Tor.Application.Common.Extensions;
using Tor.Domain.AppointmentAggregate;
using Tor.Domain.AppointmentAggregate.Enums;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.Appointments.Common;

internal sealed class AvailableTimesCalculator : IAvailableTimesCalculator
{
    private readonly ITorDbContext _context;
    private readonly IIsraelDateTimeProvider _dateTimeProvider;
    private readonly ILogger<AvailableTimesCalculator> _logger;

    public AvailableTimesCalculator(
        ITorDbContext context,
        IIsraelDateTimeProvider dateTimeProvider,
        ILogger<AvailableTimesCalculator> logger)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<List<TimeRange>> CalculateAvailableTimes(
        DateOnly atDate,
        TimeSpan duration,
        BusinessSettings settings,
        WeeklySchedule weeklySchedule,
        Guid staffMemberId,
        CancellationToken cancellationToken = default)
    {
        DateTime now = _dateTimeProvider.Now;
        DateOnly nowAsDate = DateOnly.FromDateTime(now);
        TimeOnly nowAsTime = TimeOnly.FromDateTime(now);

        // first we check if the date is a past date (cant schedule in the past)
        if (atDate < nowAsDate)
        {
            _logger.LogInformation("the atDate is a past date. atDate: {atDate}. now: {now}", atDate, now);
            return [];
        }

        // if atDate is greater than the maximum booking date in advance => cannot schedule appointment in that date
        DateOnly maximumBookingDate = nowAsDate.AddDays(settings.BookingMaximumTimeInAdvanceInDays);
        if (atDate > maximumBookingDate)
        {
            _logger.LogInformation("atDate is greater than the maximum booking date in advance." +
                " atDate: {atDate}." +
                " maximum days in advance: {maximumDaysInAdvance}", atDate, settings.BookingMaximumTimeInAdvanceInDays);
            return [];
        }

        // we get the daily schedule for the at date
        DailySchedule dailySchedule = await GetDailySchedule(atDate, weeklySchedule, staffMemberId, cancellationToken);
        if (!dailySchedule.IsWorkingDay)
        {
            _logger.LogInformation("the daily schedule is not a working day for atDate {atDate}", atDate);
            return [];
        }

        // build time sections based on the service duration and working hours
        // e.g DailySchedule => 08:00 - 16:00, Duration => 1 hour
        // Sections => [08:00, 09:00], [09:00, 10:00] ..)
        IEnumerable<TimeRange> sections = AppointmentUtils.BuildSections(dailySchedule.TimeRange!, duration);
        _logger.LogInformation("initial sections for atDate {atDate} and duration {duration}: {@sections}", atDate, duration, sections);

        // if atDate is today, we filter all past timeframes
        if (atDate == nowAsDate)
        {
            _logger.LogInformation("the atDate {atDate} is today. filtering past time frames", atDate);
            sections = sections.Where(x => x.StartTime >= nowAsTime);
        }

        // now we filter all timeframes based on the minimum booking time in advance
        // we get the threshold by adding the minimum time in minutes to now
        // if the date given is greater than atDate => the atDate is burned (return empty list)
        // if the date given is equal to atDate => filter all timeframes below the minimum threshold
        DateTime nowWithMinimumBookingTime = now.AddMinutes(settings.BookingMinimumTimeInAdvanceInMinutes);
        if (DateOnly.FromDateTime(nowWithMinimumBookingTime) > atDate)
        {
            _logger.LogInformation(
                "atDate {atDate} is burned." +
                " exceeded minimum booking time in advance: {minimumBookingTimeInAdvance}",
                atDate,
                settings.BookingMinimumTimeInAdvanceInMinutes);
            return [];
        }
        if (DateOnly.FromDateTime(nowWithMinimumBookingTime) == atDate)
        {
            _logger.LogInformation(
                "the atDate {atDate} with minimum booking time in advance {minimumBookingTimeInAdvance} is today." +
                " filtering sections accordingly", atDate, settings.BookingMinimumTimeInAdvanceInMinutes);

            sections = sections.Where(x => x.StartTime >= TimeOnly.FromDateTime(nowWithMinimumBookingTime));

            _logger.LogInformation("sections for atDate {atDate} and duration {duration}" +
                "after filtering by minimum booking time in advance: {@sections}", atDate, duration, sections);
        }

        // we filter recurring breaks
        List<TimeRange> recurringBreaksTimeRanges = dailySchedule.RecurringBreaks.ConvertAll(x => x.TimeRange);
        sections = AppointmentUtils.FilterOverlapTimeRanges(sections, recurringBreaksTimeRanges);

        _logger.LogInformation("sections for atDate {atDate} and duration {duration}" +
            "after filtering by recurring breaks: {@sections}", atDate, duration, sections);

        // we filter reserved time slots
        List<TimeRange> reservedTimeSlotsTimeRanges = await _context.ReservedTimeSlots
            .AsNoTracking()
            .Where(x => x.StaffMemberId == staffMemberId)
            .Where(x => x.AtDate == atDate)
            .Select(x => x.TimeRange)
            .ToListAsync(cancellationToken);
        sections = AppointmentUtils.FilterOverlapTimeRanges(sections, reservedTimeSlotsTimeRanges);

        _logger.LogInformation("sections for atDate {atDate} and duration {duration}" +
            "after filtering by recurring breaks: {@sections}", atDate, duration, sections);

        if (!sections.Any())
        {
            return [];
        }

        // we filter all sections that overlap with existing appointments
        List<TimeRange> appointmentTimeRanges = await GetAppointments(atDate, staffMemberId, cancellationToken);

        _logger.LogInformation("existing appointments for staff member with id {staffMemberId} by atDate {atDate}: {@appointments}",
            staffMemberId,
            atDate,
            appointmentTimeRanges);

        sections = AppointmentUtils.FilterOverlapTimeRanges(sections, appointmentTimeRanges);

        _logger.LogInformation("sections for atDate {atDate} and duration {duration}" +
            "after filtering by existing appointments: {@sections}", atDate, duration, sections);

        return sections.ToList();
    }

    private async Task<List<TimeRange>> GetAppointments(DateOnly atDate, Guid staffMemberId, CancellationToken cancellationToken)
    {
        DateTime filterDate = _dateTimeProvider.Now.AddDays(-1);
        List<Appointment> existingAppointments = await _context.Appointments
            .AsNoTracking()
            .Where(x => x.StaffMemberId == staffMemberId)
            .Where(x => x.ScheduledFor >= filterDate)
            .Where(x => x.Status != AppointmentStatusType.Canceled)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("unfiltered appointments: {@appointments}", existingAppointments);

        var filteredAppointments = existingAppointments.Where(x =>
                x.ScheduledFor.ToIsraelTime() > atDate.ToDateTime(new TimeOnly(0, 0)) &&
                x.ScheduledFor.ToIsraelTime() < atDate.ToDateTime(new TimeOnly(23, 59, 59)))
            .ToList();

        _logger.LogInformation("filtered appointments: {@appointments}", filteredAppointments);

        return filteredAppointments.ConvertAll(x =>
        {
            TimeSpan duration = TimeSpan.FromMinutes(x.ServiceDetails.Durations.First().ValueInMinutes);
            DateTime scheduledForAsIsraelTime = x.ScheduledFor.ToIsraelTime();

            TimeOnly start = TimeOnly.FromDateTime(scheduledForAsIsraelTime);
            TimeOnly end = TimeOnly.FromDateTime(scheduledForAsIsraelTime + duration);

            return new TimeRange(start, end);
        });
    }

    private async Task<DailySchedule> GetDailySchedule(
        DateOnly atDate,
        WeeklySchedule weeklySchedule,
        Guid staffMemberId,
        CancellationToken cancellationToken)
    {
        List<CustomWorkingDay> customWorkingDays = await _context.StaffMembers
            .AsNoTracking()
            .Where(x => x.Id == staffMemberId)
            .Select(x => x.CustomWorkingDays)
            .FirstAsync(cancellationToken);

        CustomWorkingDay? staffMemberCustomWorkingDay = customWorkingDays.FirstOrDefault(x => x.AtDate == atDate);
        if (staffMemberCustomWorkingDay is not null)
        {
            return staffMemberCustomWorkingDay.DailySchedule;
        }

        return weeklySchedule.GetDailySchedule(atDate.DayOfWeek);
    }
}
