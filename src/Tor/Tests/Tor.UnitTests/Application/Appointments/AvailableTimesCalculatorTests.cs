using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Tor.Application.Abstractions;
using Tor.Application.Appointments.Common;
using Tor.Application.Common.Extensions;
using Tor.Domain.AppointmentAggregate;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.BusinessAggregate.Enums;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.Common.ValueObjects;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;
using Tor.TestsInfra.Utils;

namespace Tor.UnitTests.Application.Appointments;

public class AvailableTimesCalculatorTests : UnitTestBase
{
    private readonly AvailableTimesCalculator _sut;
    private readonly Mock<IIsraelDateTimeProvider> _dateTimeProviderMock;
    private readonly Mock<ILogger<AvailableTimesCalculator>> _loggerMock;
    private readonly DateTime _defaultDateTime = new(2024, 1, 1, 6, 0, 0);

    public AvailableTimesCalculatorTests()
        : base()
    {
        _dateTimeProviderMock = new Mock<IIsraelDateTimeProvider>();
        _dateTimeProviderMock.Setup(x => x.Now).Returns(_defaultDateTime);

        _loggerMock = new Mock<ILogger<AvailableTimesCalculator>>();

        _sut = new AvailableTimesCalculator(
            Context,
            _dateTimeProviderMock.Object,
            _loggerMock.Object);
    }

    // working hours => 6 - 23
    // recurring break => 9 - 10
    // service duration => 1 hour
    // reserved time slot => 18-19
    // existing appointment => 14 -15
    // minimum booking time in advance => 1 hour
    [Fact]
    public async Task CalculateAvailableTimes_WhenValidRequest_ShouldGetAvailableTimes()
    {
        Service service = Fakers.Services.ServiceFaker.Generate();
        service.Durations = [new Duration(1, 60, DurationType.Work)];
        service.StaffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        service.StaffMember.Business = Fakers.Businesses.BusinessFaker.Generate();
        SetValidTimeRangesForWeeklySchedule(service.StaffMember.WeeklySchedule);
        service.StaffMember.ReservedTimeSlots.Add(new ReservedTimeSlot(Guid.NewGuid())
        {
            AtDate = DateOnly.FromDateTime(_defaultDateTime),
            TimeRange = new(new(18, 0), new(19, 0)),
        });
        await Context.Services.AddAsync(service);
        await Context.SaveChangesAsync();
        Appointment appointment = Fakers.Appointments.AppointmentFaker.Generate();
        appointment.ServiceDetails.Durations = [new Duration(1, 60, DurationType.Work)];
        appointment.ScheduledFor = new DateTime(DateOnly.FromDateTime(_defaultDateTime), new TimeOnly(12, 0), DateTimeKind.Utc);
        appointment.StaffMemberId = service.StaffMemberId;
        await Context.Appointments.AddAsync(appointment);
        await Context.SaveChangesAsync();

        var result = await _sut.CalculateAvailableTimes(
            DateOnly.FromDateTime(_defaultDateTime),
            TimeSpan.FromMinutes(service.Durations.First().ValueInMinutes),
            service.StaffMember.Business.Settings,
            service.StaffMember.WeeklySchedule,
            service.StaffMemberId);

        result.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(new List<TimeRange>()
        {
            new (new(7, 0),new(8, 0)),
            new (new(8, 0),new(9, 0)),
            new (new(10, 0),new(11, 0)),
            new (new(11, 0),new(12, 0)),
            new (new(12, 0),new(13, 0)),
            new (new(13, 0),new(14, 0)),
            new (new(15, 0),new(16, 0)),
            new (new(16, 0),new(17, 0)),
            new (new(17, 0),new(18, 0)),
            new (new(19, 0),new(20, 0)),
            new (new(20, 0),new(21, 0)),
            new (new(21, 0),new(22, 0)),
            new (new(22, 0),new(23, 0)),
        });
    }

    [Fact]
    public async Task CalculateAvailableTimes_WhenDayIsFullyBookedWithAppointments_ShouldReturnEmptyList()
    {
        Service service = Fakers.Services.ServiceFaker.Generate();
        service.StaffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        service.StaffMember.Business = Fakers.Businesses.BusinessFaker.Generate();
        SetValidTimeRangesForWeeklySchedule(service.StaffMember.WeeklySchedule, withRecurringBreaks: false);
        await Context.Services.AddAsync(service);
        await Context.SaveChangesAsync();
        List<Appointment> appointments = Fakers.Appointments.AppointmentFaker.Generate(200);
        int minutes = 10;
        foreach (var appointment in appointments)
        {
            appointment.ScheduledFor = DateTime.UtcNow.Date.AddMinutes(minutes);
            appointment.StaffMemberId = service.StaffMember.Id;
            minutes += 10;
        }
        await Context.Appointments.AddRangeAsync(appointments);
        await Context.SaveChangesAsync();

        var result = await _sut.CalculateAvailableTimes(
            DateOnly.FromDateTime(DateTime.UtcNow),
            TimeSpan.FromMinutes(service.Durations.First().ValueInMinutes),
            service.StaffMember.Business.Settings,
            service.StaffMember.WeeklySchedule,
            service.StaffMemberId);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CalculateAvailableTimes_WhenReservedTimeSlotsAreAllDay_ShouldReturnEmptyList()
    {
        Service service = Fakers.Services.ServiceFaker.Generate();
        service.StaffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        service.StaffMember.Business = Fakers.Businesses.BusinessFaker.Generate();
        SetValidTimeRangesForWeeklySchedule(service.StaffMember.WeeklySchedule, withRecurringBreaks: false);
        service.StaffMember.ReservedTimeSlots.Add(new ReservedTimeSlot(Guid.NewGuid())
        {
            AtDate = DateOnly.FromDateTime(_defaultDateTime),
            TimeRange = new(
                service.StaffMember.WeeklySchedule.Sunday.TimeRange!.StartTime,
                service.StaffMember.WeeklySchedule.Sunday.TimeRange!.EndTime),
        });
        await Context.Services.AddAsync(service);
        await Context.SaveChangesAsync();

        var result = await _sut.CalculateAvailableTimes(
            DateOnly.FromDateTime(_defaultDateTime),
            TimeSpan.FromMinutes(service.Durations.First().ValueInMinutes),
            service.StaffMember.Business.Settings,
            service.StaffMember.WeeklySchedule,
            service.StaffMemberId);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CalculateAvailableTimes_WhenMinimumBookingTimeInAdvanceFiltersAllAvailableTimes_ShouldReturnEmptyList()
    {
        Service service = Fakers.Services.ServiceFaker.Generate();
        service.Durations = [new Duration(1, 60, DurationType.Work)];
        service.StaffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        service.StaffMember.Business = Fakers.Businesses.BusinessFaker.Generate();
        service.StaffMember.Business.Settings = new(1440, 14, 100, 100, 3, 24);
        SetValidTimeRangesForWeeklySchedule(service.StaffMember.WeeklySchedule);
        await Context.Services.AddAsync(service);
        await Context.SaveChangesAsync();

        var result = await _sut.CalculateAvailableTimes(
            DateOnly.FromDateTime(_defaultDateTime),
            TimeSpan.FromMinutes(service.Durations.First().ValueInMinutes),
            service.StaffMember.Business.Settings,
            service.StaffMember.WeeklySchedule,
            service.StaffMemberId);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CalculateAvailableTimes_WhenAtDateIsPastDate_ShouldReturnEmptyList()
    {
        Service service = Fakers.Services.ServiceFaker.Generate();
        service.Durations = [new Duration(1, 60, DurationType.Work)];
        service.StaffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        service.StaffMember.Business = Fakers.Businesses.BusinessFaker.Generate();
        await Context.Services.AddAsync(service);
        await Context.SaveChangesAsync();

        var result = await _sut.CalculateAvailableTimes(
            DateOnly.FromDateTime(_defaultDateTime.AddDays(-1)),
            TimeSpan.FromMinutes(service.Durations.First().ValueInMinutes),
            service.StaffMember.Business.Settings,
            service.StaffMember.WeeklySchedule,
            service.StaffMemberId);

        result.Should().BeEmpty();
        _loggerMock.VerifyLogContains("the atDate is a past date.");
    }

    [Theory]
    [InlineData(14)]
    public async Task CalculateAvailableTimes_WhenAtDateIsOutsideMaximumBookingTimeInAdvance_ShouldReturnEmptyList(int maximumBookingTimeInAdvance)
    {
        Service service = Fakers.Services.ServiceFaker.Generate();
        service.Durations = [new Duration(1, 60, DurationType.Work)];
        service.StaffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        service.StaffMember.Business = Fakers.Businesses.BusinessFaker.Generate();
        service.StaffMember.Business.Settings = new(1, maximumBookingTimeInAdvance, 100, 100, 3, 24);
        SetValidTimeRangesForWeeklySchedule(service.StaffMember.WeeklySchedule);
        await Context.Services.AddAsync(service);
        await Context.SaveChangesAsync();

        var result = await _sut.CalculateAvailableTimes(
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(maximumBookingTimeInAdvance + 1)),
            TimeSpan.FromMinutes(service.Durations.First().ValueInMinutes),
            service.StaffMember.Business.Settings,
            service.StaffMember.WeeklySchedule,
            service.StaffMemberId);

        result.Should().BeEmpty();
        _loggerMock.VerifyLogContains("atDate is greater than the maximum booking date in advance.");
    }

    [Fact]
    public async Task CalculateAvailableTimes_WhenDayIsNotWorkingDay_ShouldReturnValidationError()
    {
        DateOnly atDate = DateOnly.FromDateTime(_defaultDateTime);
        Service service = Fakers.Services.ServiceFaker.Generate();
        service.StaffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        service.StaffMember.Business = Fakers.Businesses.BusinessFaker.Generate();
        SetValidTimeRangesForWeeklySchedule(service.StaffMember.WeeklySchedule, isWorkingDays: true);
        DailySchedule dailySchedule = service.StaffMember.WeeklySchedule.GetDailySchedule(atDate.DayOfWeek);
        dailySchedule.IsWorkingDay = false;
        await Context.Services.AddAsync(service);
        await Context.SaveChangesAsync();

        var result = await _sut.CalculateAvailableTimes(
            atDate,
            TimeSpan.FromMinutes(service.Durations.First().ValueInMinutes),
            service.StaffMember.Business.Settings,
            service.StaffMember.WeeklySchedule,
            service.StaffMemberId);

        result.Should().BeEmpty();
        _loggerMock.VerifyLogContains($"the daily schedule is not a working day for atDate {atDate}");

    }

    [Fact]
    public async Task CalculateAvailableTimes_WhenStaffMemberCustomWorkingDayIsNotWorkingDay_ShouldReturnValidationError()
    {
        DateOnly atDate = DateOnly.FromDateTime(_defaultDateTime);
        Service service = Fakers.Services.ServiceFaker.Generate();
        service.StaffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        service.StaffMember.Business = Fakers.Businesses.BusinessFaker.Generate();
        SetValidTimeRangesForWeeklySchedule(service.StaffMember.WeeklySchedule, isWorkingDays: true);
        service.StaffMember.CustomWorkingDays.Add(new CustomWorkingDay
        {
            AtDate = atDate,
            DailySchedule = new DailySchedule(false, null, null!),
        });
        await Context.Services.AddAsync(service);
        await Context.SaveChangesAsync();

        var result = await _sut.CalculateAvailableTimes(
            atDate,
            TimeSpan.FromMinutes(service.Durations.First().ValueInMinutes),
            service.StaffMember.Business.Settings,
            service.StaffMember.WeeklySchedule,
            service.StaffMemberId);

        result.Should().BeEmpty();
        _loggerMock.VerifyLogContains($"the daily schedule is not a working day for atDate {atDate}");
    }

    private void SetValidTimeRangesForWeeklySchedule(WeeklySchedule weeklySchedule, bool isWorkingDays = true, bool withRecurringBreaks = true)
    {
        weeklySchedule.Sunday.IsWorkingDay = isWorkingDays;
        weeklySchedule.Monday.IsWorkingDay = isWorkingDays;
        weeklySchedule.Tuesday.IsWorkingDay = isWorkingDays;
        weeklySchedule.Wednesday.IsWorkingDay = isWorkingDays;
        weeklySchedule.Thursday.IsWorkingDay = isWorkingDays;
        weeklySchedule.Friday.IsWorkingDay = isWorkingDays;
        weeklySchedule.Saturday.IsWorkingDay = isWorkingDays;

        weeklySchedule.Sunday.TimeRange = new TimeRange(new(6, 0), new(23, 0));
        weeklySchedule.Monday.TimeRange = new TimeRange(new(6, 0), new(23, 0));
        weeklySchedule.Tuesday.TimeRange = new TimeRange(new(6, 0), new(23, 0));
        weeklySchedule.Wednesday.TimeRange = new TimeRange(new(6, 0), new(23, 0));
        weeklySchedule.Thursday.TimeRange = new TimeRange(new(6, 0), new(23, 0));
        weeklySchedule.Friday.TimeRange = new TimeRange(new(6, 0), new(23, 0));
        weeklySchedule.Saturday.TimeRange = new TimeRange(new(6, 0), new(23, 0));

        weeklySchedule.Sunday.RecurringBreaks = [new RecurringBreak(1, new TimeRange(new(9, 0), new(10, 0)))];
        weeklySchedule.Monday.RecurringBreaks = [new RecurringBreak(1, new TimeRange(new(9, 0), new(10, 0)))];
        weeklySchedule.Tuesday.RecurringBreaks = [new RecurringBreak(1, new TimeRange(new(9, 0), new(10, 0)))];
        weeklySchedule.Wednesday.RecurringBreaks = [new RecurringBreak(1, new TimeRange(new(9, 0), new(10, 0)))];
        weeklySchedule.Thursday.RecurringBreaks = [new RecurringBreak(1, new TimeRange(new(9, 0), new(10, 0)))];
        weeklySchedule.Friday.RecurringBreaks = [new RecurringBreak(1, new TimeRange(new(9, 0), new(10, 0)))];
        weeklySchedule.Saturday.RecurringBreaks = [new RecurringBreak(1, new TimeRange(new(9, 0), new(10, 0)))];

        if (!withRecurringBreaks)
        {
            weeklySchedule.Sunday.RecurringBreaks.Clear();
            weeklySchedule.Monday.RecurringBreaks.Clear();
            weeklySchedule.Tuesday.RecurringBreaks.Clear();
            weeklySchedule.Wednesday.RecurringBreaks.Clear();
            weeklySchedule.Thursday.RecurringBreaks.Clear();
            weeklySchedule.Friday.RecurringBreaks.Clear();
            weeklySchedule.Saturday.RecurringBreaks.Clear();
        }
    }
}
