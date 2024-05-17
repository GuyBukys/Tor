using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tor.Domain.ClientAggregate.Entities;

namespace Tor.Infrastructure.Configurations;

public class FavoriteBusinessConfigurations : IEntityTypeConfiguration<FavoriteBusiness>
{
    public void Configure(EntityTypeBuilder<FavoriteBusiness> builder)
    {
        ConfigureFavoriteBusinessesTable(builder);
    }

    private static void ConfigureFavoriteBusinessesTable(EntityTypeBuilder<FavoriteBusiness> builder)
    {
        builder.ToTable("FavoriteBusinesses");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasOne(x => x.Client)
            .WithMany()
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Business)
            .WithMany()
            .HasForeignKey(x => x.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x => new { x.BusinessId, x.ClientId })
            .IsUnique();

        builder.Property(x => x.MuteNotifications)
            .IsRequired()
            .HasDefaultValue(false);
    }
}
