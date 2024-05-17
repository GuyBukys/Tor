using FluentAssertions;
using Tor.Application.Clients.Queries.GetAppointments;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.AppointmentAggregate;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.ClientAggregate;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Clients;

public class GetClientAppointmentsQueryHandlerTests : UnitTestBase
{
    private readonly GetClientAppointmentsQueryHandler _sut;

    public GetClientAppointmentsQueryHandlerTests()
        : base()
    {
        _sut = new GetClientAppointmentsQueryHandler(Context);
    }

    [Fact]
    public async Task Handle_WhenValidRequestAndAreNotCancellable_ShouldGetClientAppointmentsWithIsCancellableFalse()
    {
        var business = Fakers.Businesses.BusinessFaker.Generate();
        business.Settings = business.Settings with
        {
            CancelAppointmentMinimumTimeInMinutes = int.MaxValue,
        };
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        staffMember.Business = business;
        await Context.StaffMembers.AddAsync(staffMember);
        await Context.SaveChangesAsync();
        Guid clientId = await SetupClient();
        List<Appointment> appointments = await SetupAppointments(staffMember, clientId);
        GetClientAppointmentsQuery query = new(clientId);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        appointments.Should().BeEquivalentTo(result.Value, cfg => cfg.ExcludingMissingMembers());
        result.Value.Select(x => x.StaffMemberName).Should().AllBe(staffMember.Name);
        appointments.Select(x => x.StaffMember.Business).Should().BeEquivalentTo(result.Value.Select(x => x.BusinessDetails), cfg => cfg.ExcludingMissingMembers());
        result.Value.Select(x => x.IsCancellable).Should().AllBeEquivalentTo(false);
        result.Value.Should().BeInAscendingOrder(x => x.ScheduledFor);
    }

    [Fact]
    public async Task Handle_WhenAllAppointmentsInThePast_ShouldReturnEmptyList()
    {
        var business = Fakers.Businesses.BusinessFaker.Generate();
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        staffMember.Business = business;
        await Context.StaffMembers.AddAsync(staffMember);
        await Context.SaveChangesAsync();
        Guid clientId = await SetupClient();
        List<Appointment> appointments = await SetupAppointments(staffMember, clientId);
        appointments.ForEach(x => x.ScheduledFor = DateTimeOffset.UtcNow.AddDays(-1));
        await Context.SaveChangesAsync();
        GetClientAppointmentsQuery query = new(clientId);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenValidRequestAndAreCancellable_ShouldGetClientAppointmentsWithIsCancellableTrue()
    {
        var business = Fakers.Businesses.BusinessFaker.Generate();
        business.Settings = business.Settings with
        {
            CancelAppointmentMinimumTimeInMinutes = 1,
        };
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        staffMember.Business = business;
        await Context.StaffMembers.AddAsync(staffMember);
        await Context.SaveChangesAsync();
        Guid clientId = await SetupClient();
        List<Appointment> appointments = await SetupAppointments(staffMember, clientId);
        GetClientAppointmentsQuery query = new(clientId);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        result.Value.Select(x => x.StaffMemberName).Should().AllBe(staffMember.Name);
        appointments.Should().BeEquivalentTo(result.Value, cfg => cfg.ExcludingMissingMembers());
        appointments.Select(x => x.StaffMember.Business).Should().BeEquivalentTo(result.Value.Select(x => x.BusinessDetails), cfg => cfg.ExcludingMissingMembers());
        result.Value.Select(x => x.IsCancellable).Should().AllBeEquivalentTo(true);
    }

    [Fact]
    public async Task Handle_WhenClientDoesntExist_ShouldReturnFailedResult()
    {
        GetClientAppointmentsQuery query = new(Guid.NewGuid());

        var result = await _sut.Handle(query, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(
            x => x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find client with id {query.ClientId}"));
    }

    private async Task<Guid> SetupClient()
    {
        Client client = Fakers.Clients.ClientFaker.Generate();

        await Context.Clients.AddAsync(client);
        await Context.SaveChangesAsync();
        return client.Id;
    }

    private async Task<List<Appointment>> SetupAppointments(StaffMember staffMember, Guid clientId)
    {
        int day = 1;
        List<Appointment> appointments = Fakers.Appointments.AppointmentFaker.Generate(20);
        appointments.ForEach(x =>
        {
            x.StaffMember = staffMember;
            x.ClientId = clientId;
            x.ScheduledFor = DateTimeOffset.UtcNow.AddDays(day);
            day++;
        });

        await Context.Appointments.AddRangeAsync(appointments);
        await Context.SaveChangesAsync();

        return appointments;
    }
}
