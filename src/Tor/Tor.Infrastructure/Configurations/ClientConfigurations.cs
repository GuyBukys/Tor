using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.ClientAggregate;
using Tor.Domain.ClientAggregate.Entities;
using Tor.Infrastructure.Utils;

namespace Tor.Infrastructure.Configurations;

public class ClientConfigurations : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        ConfigureClientsTable(builder);
    }

    private static void ConfigureClientsTable(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<Client>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(x => x.ProfileImage);

        builder.HasMany(x => x.Appointments)
            .WithOne(x => x.Client)
            .HasForeignKey(x => x.ClientId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Businesses)
            .WithMany(x => x.Clients)
            .UsingEntity<BusinessClient>();

        builder.HasMany<Business>()
            .WithMany(x => x.BlockedClients)
            .UsingEntity<BlockedClient>();

        builder.HasMany<FavoriteBusiness>()
            .WithOne(x => x.Client)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.BirthDate)
            .IsRequired(false)
            .HasConversion<DateOnlyConverter, DateOnlyComparer>();

        builder.OwnsOne(x => x.PhoneNumber);

        builder.OwnsOne(x => x.Address);
    }
}
