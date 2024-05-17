using FluentAssertions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.Common.Extensions;
using Tor.Application.StaffMembers.Queries.GetSchedule;
using Tor.Domain.AppointmentAggregate;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.StaffMembers;

public class GetScheduleQueryHandlerTests : UnitTestBase
{
    private readonly GetScheduleQueryHandler _sut;

    public GetScheduleQueryHandlerTests()
        : base()
    {
        _sut = new GetScheduleQueryHandler(Context);
    }

    [Fact]
    public async Task Handle_WhenAppointmentsAndRtsExistsWithNoUntilDate_ShouldReturnAppointmentsAndRtsUntilOneWeekFromFromDate()
    {
        Guid staffMemberId = await SetupStaffMember();
        List<Appointment> appointments = await SetupAppointments(staffMemberId);
        List<ReservedTimeSlot> reservedTimeSlots = await SetupReservedTimeSlots(staffMemberId);
        GetScheduleQuery query = new(
            staffMemberId,
            From: DateTime.UtcNow.AddDays(10),
            Until: null);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Appointments.Should().NotBeEmpty();
        DateTimeOffset until = query.From!.Value.AddDays(7);
        var appointmentsOneWeekFromFromdate = appointments.Where(x => x.ScheduledFor >= query.From && x.ScheduledFor <= until);
        result.Value.Appointments.Should().BeEquivalentTo(appointmentsOneWeekFromFromdate);
        var reservedTimeSlotsOneWeekFromFromdate = reservedTimeSlots.Where(
            x => x.AtDate >= DateOnly.FromDateTime(query.From.Value.ToIsraelTime()) && x.AtDate <= DateOnly.FromDateTime(until.ToIsraelTime()));
        result.Value.ReservedTimeSlots.Should().BeEquivalentTo(reservedTimeSlotsOneWeekFromFromdate);
    }

    [Fact]
    public async Task Handle_WhenAppointmentsAndRtsExistsWithNoFromDate_ShouldReturnAppointmentsAndRtsFromNow()
    {
        Guid staffMemberId = await SetupStaffMember();
        List<Appointment> appointments = await SetupAppointments(staffMemberId);
        List<ReservedTimeSlot> reservedTimeSlots = await SetupReservedTimeSlots(staffMemberId);
        GetScheduleQuery query = new(
            staffMemberId,
            From: null,
            Until: DateTime.UtcNow.AddYears(1));

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Appointments.Should().NotBeEmpty();
        var appointmentsFromNow = appointments.Where(x => x.ScheduledFor.UtcDateTime >= DateTime.UtcNow);
        result.Value.Appointments.Should().BeEquivalentTo(appointmentsFromNow);
        var reservedTimeSlotsFromNow = reservedTimeSlots.Where(x => x.AtDate >= DateOnly.FromDateTime(DateTimeOffset.UtcNow.ToIsraelTime()));
        result.Value.ReservedTimeSlots.Should().BeEquivalentTo(reservedTimeSlotsFromNow);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ShouldReturnAppointmentsAndRtsFromDatesGiven()
    {
        Guid staffMemberId = await SetupStaffMember();
        List<Appointment> appointments = await SetupAppointments(staffMemberId);
        List<ReservedTimeSlot> reservedTimeSlots = await SetupReservedTimeSlots(staffMemberId);
        GetScheduleQuery query = new(
            staffMemberId,
            From: DateTime.UtcNow.AddDays(5),
            Until: DateTime.UtcNow.AddDays(10));

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Appointments.Should().NotBeEmpty();
        var appointmentsInDateRange = appointments.Where(x => x.ScheduledFor >= query.From && x.ScheduledFor <= query.Until);
        result.Value.Appointments.Should().BeEquivalentTo(appointmentsInDateRange);
        var reservedTimeSlotsInDateRange = reservedTimeSlots.Where(x =>
            x.AtDate >= DateOnly.FromDateTime(query.From!.Value.ToIsraelTime()) && x.AtDate <= DateOnly.FromDateTime(query.Until!.Value.ToIsraelTime()));
        result.Value.ReservedTimeSlots.Should().BeEquivalentTo(reservedTimeSlotsInDateRange);
    }

    [Fact]
    public async Task Handle_WhenAppointmentsAndRtsExistsWithOnlyUntilDate_ShouldReturnAppointmentsAndRtsUntilTheDateGiven()
    {
        Guid staffMemberId = await SetupStaffMember();
        List<Appointment> appointments = await SetupAppointments(staffMemberId);
        List<ReservedTimeSlot> reservedTimeSlots = await SetupReservedTimeSlots(staffMemberId);
        GetScheduleQuery query = new(staffMemberId, From: null, Until: DateTime.UtcNow.AddYears(2));

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Appointments.Should().NotBeEmpty();
        result.Value.Appointments.Should().BeEquivalentTo(appointments);
        result.Value.ReservedTimeSlots.Should().BeEquivalentTo(reservedTimeSlots);
    }

    [Fact]
    public async Task Handle_WhenNoScheduleExists_ShouldReturnEmptyLists()
    {
        Guid staffMemberId = await SetupStaffMember();
        GetScheduleQuery query = new(staffMemberId, null, null);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Appointments.Should().BeEmpty();
        result.Value.ReservedTimeSlots.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenStaffMemberDoesntExist_ShouldReturnFailedResult()
    {
        GetScheduleQuery query = new(Guid.NewGuid(), null, null);

        var result = await _sut.Handle(query, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(
            x => x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find staff member with id {query.StaffMemberId}"));
    }

    private async Task<Guid> SetupStaffMember()
    {
        StaffMember staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();

        await Context.StaffMembers.AddAsync(staffMember);
        await Context.SaveChangesAsync();
        return staffMember.Id;
    }

    private async Task<List<Appointment>> SetupAppointments(Guid staffMemberId)
    {
        int day = 1;
        List<Appointment> appointments = Fakers.Appointments.AppointmentFaker.Generate(20);
        appointments.ForEach(x =>
        {
            x.StaffMemberId = staffMemberId;
            x.ScheduledFor = DateTimeOffset.UtcNow.AddDays(day);
            day++;
        });

        await Context.Appointments.AddRangeAsync(appointments);
        await Context.SaveChangesAsync();

        return appointments;
    }

    private async Task<List<ReservedTimeSlot>> SetupReservedTimeSlots(Guid staffMemberId)
    {
        int day = 1;
        List<ReservedTimeSlot> reservedTimeSlots = Fakers.ReservedTimeSlots.ReservedTimeSlotFaker.Generate(20);
        reservedTimeSlots.ForEach(x =>
        {
            x.StaffMemberId = staffMemberId;
            x.AtDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(day));
            day++;
        });

        await Context.ReservedTimeSlots.AddRangeAsync(reservedTimeSlots);
        await Context.SaveChangesAsync();

        return reservedTimeSlots;
    }
}
