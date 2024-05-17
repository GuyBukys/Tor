using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tor.Domain.WaitingListAggregate;

namespace Tor.Infrastructure.Configurations;
public class WaitingListConfigurations : IEntityTypeConfiguration<WaitingList>
{
    public void Configure(EntityTypeBuilder<WaitingList> builder)
    {
        ConfigureTable(builder);
    }

    private static void ConfigureTable(EntityTypeBuilder<WaitingList> builder)
    {
        builder.ToTable("WaitingLists");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.AtDate)
            .IsRequired();

        builder.HasOne(x => x.StaffMember)
            .WithMany(x => x.WaitingLists)
            .HasForeignKey(x => x.StaffMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.AtDate, x.StaffMemberId })
            .IsUnique();

        builder.HasMany(x => x.Clients)
            .WithMany(x => x.WaitingLists);
    }
}
