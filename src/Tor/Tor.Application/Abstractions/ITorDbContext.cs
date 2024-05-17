using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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

namespace Tor.Application.Abstractions;

public interface ITorDbContext
{
    DbSet<Business> Businesses { get; set; }
    DbSet<Category> Categories { get; set; }
    DbSet<StaffMember> StaffMembers { get; set; }
    DbSet<Service> Services { get; set; }
    DbSet<Client> Clients { get; set; }
    DbSet<Appointment> Appointments { get; set; }
    DbSet<User> Users { get; set; }
    DbSet<Domain.TierAggregate.Tier> Tiers { get; set; }
    DbSet<WaitingList> WaitingLists { get; set; }
    DbSet<FavoriteBusiness> FavoriteBusinesses { get; set; }
    DbSet<MessageBlast> MessageBlasts { get; set; }
    DbSet<BusinessMessageBlast> BusinessMessageBlasts { get; set; }
    DbSet<BusinessClient> BusinessClients { get; set; }
    DbSet<BlockedClient> BlockedClients { get; set; }
    DbSet<ReservedTimeSlot> ReservedTimeSlots { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}
