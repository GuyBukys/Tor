using Microsoft.EntityFrameworkCore;
using Tor.Application.Common.Extensions;
using Tor.Domain.AppointmentAggregate;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.ClientAggregate;
using Tor.Domain.TierAggregate;
using Tor.Domain.UserAggregate;
using Tor.Domain.UserAggregate.Enum;
using Tor.Infrastructure.Persistence;
using Tor.TestsInfra.CustomFakers;

namespace Tor.IntegrationTests;

internal static class TestUtils
{
    internal static async Task<Business> SetupBusiness(TorDbContext context)
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();

        Tier tier = await context.Tiers.FirstAsync();
        business.TierId = tier.Id;
        business.Tier = tier;

        await context.Businesses.AddAsync(business);
        await context.SaveChangesAsync();

        return business;
    }

    internal static async Task<StaffMember> SetupStaffMember(TorDbContext context)
    {
        User user = Fakers.Users.UserFaker.Generate();
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        StaffMember staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        staffMember.UserId = user.Id;

        Business business = Fakers.Businesses.BusinessFaker.Generate();
        business.StaffMembers = [staffMember];

        Tier tier = await context.Tiers.FirstAsync();
        business.TierId = tier.Id;
        business.Tier = tier;

        await context.Businesses.AddAsync(business);
        await context.SaveChangesAsync();

        return await context.StaffMembers
            .Include(x => x.Business)
            .ThenInclude(x => x.Tier)
            .FirstAsync(x => x.Id == staffMember.Id);
    }

    internal static async Task<Service> SetupService(Guid staffMemberId, TorDbContext context)
    {
        Service service = Fakers.Services.ServiceFaker.Generate();
        service.StaffMemberId = staffMemberId;

        await context.Services.AddAsync(service);
        await context.SaveChangesAsync();

        return service;
    }

    internal static async Task<Client> SetupClient(TorDbContext context)
    {
        Client client = Fakers.Clients.ClientFaker.Generate();
        client.User.EntityId = client.Id;

        await context.Clients.AddAsync(client);
        await context.SaveChangesAsync();

        return client;
    }

    internal static async Task<List<Appointment>> SetupAppointments(StaffMember staffMember, TorDbContext context, Guid? clientId = null)
    {
        var client = Fakers.Clients.ClientFaker.Generate();
        await context.Clients.AddAsync(client);
        await context.SaveChangesAsync();

        int day = 1;
        List<Appointment> appointments = Fakers.Appointments.AppointmentFaker.Generate(20);
        appointments.ForEach(x =>
        {
            x.ClientId = clientId ?? client.Id;
            x.Client = clientId is null ? Fakers.Clients.ClientFaker.Generate() : null;
            x.StaffMemberId = staffMember.Id;
            x.StaffMember = staffMember;
            x.ScheduledFor = DateTimeOffset.UtcNow.AddDays(day);
            day++;
        });

        await context.Appointments.AddRangeAsync(appointments);
        await context.SaveChangesAsync();

        return appointments;
    }

    internal static async Task<List<ReservedTimeSlot>> SetupReservedTimeSlots(StaffMember staffMember, TorDbContext context)
    {
        var client = Fakers.Clients.ClientFaker.Generate();
        await context.Clients.AddAsync(client);
        await context.SaveChangesAsync();

        int day = 1;
        List<ReservedTimeSlot> reservedTimeSlots = Fakers.ReservedTimeSlots.ReservedTimeSlotFaker.Generate(20);
        reservedTimeSlots.ForEach(x =>
        {
            x.StaffMemberId = staffMember.Id;
            x.AtDate = DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddDays(day).ToIsraelTime());
            day++;
        });

        await context.ReservedTimeSlots.AddRangeAsync(reservedTimeSlots);
        await context.SaveChangesAsync();

        return reservedTimeSlots;
    }

    internal static async Task<Guid> SetupUser(TorDbContext context)
    {
        var user = Fakers.Users.UserFaker.Generate();
        user.AppType = AppType.BusinessApp;

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        return user.Id;
    }

    internal static void SetupUserTokenHeader(HttpClient client, string userToken)
    {
        client.DefaultRequestHeaders.Add(Infrastructure.Constants.UserTokenHeaderName, userToken);
    }
}
