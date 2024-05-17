using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Tor.Application.Abstractions;
using Tor.Domain.AppointmentAggregate;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.CategoryAggregate;
using Tor.Domain.ClientAggregate;
using Tor.Domain.ClientAggregate.Entities;
using Tor.Domain.MessageBlastAggregate;
using Tor.Domain.MessageBlastAggregate.Entities;
using Tor.Domain.TierAggregate;
using Tor.Domain.UserAggregate;
using Tor.Domain.WaitingListAggregate;
using Tor.Infrastructure.Utils;

namespace Tor.Infrastructure.Persistence;

internal sealed class TorDbContext : DbContext, ITorDbContext
{
    public TorDbContext(DbContextOptions<TorDbContext> options)
        : base(options)
    {

    }

    public DbSet<Business> Businesses { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<StaffMember> StaffMembers { get; set; } = null!;
    public DbSet<Service> Services { get; set; } = null!;
    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<Appointment> Appointments { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Domain.TierAggregate.Tier> Tiers { get; set; } = null!;
    public DbSet<WaitingList> WaitingLists { get; set; } = null!;
    public DbSet<FavoriteBusiness> FavoriteBusinesses { get; set; } = null!;
    public DbSet<MessageBlast> MessageBlasts { get; set; } = null!;
    public DbSet<BusinessMessageBlast> BusinessMessageBlasts { get; set; } = null!;
    public DbSet<BusinessClient> BusinessClients { get; set; } = null!;
    public DbSet<BlockedClient> BlockedClients { get; set; } = null!;
    public DbSet<ReservedTimeSlot> ReservedTimeSlots { get; set; } = null!;

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        return Database.BeginTransactionAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TorDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder
            .Properties<TimeOnly>()
            .HaveConversion<TimeOnlyConverter, TimeOnlyComparer>()
            .HaveColumnType("time without time zone");
    }
}
