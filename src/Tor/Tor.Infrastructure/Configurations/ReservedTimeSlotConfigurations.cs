using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tor.Domain.BusinessAggregate.Entities;

namespace Tor.Infrastructure.Configurations;

public class ReservedTimeSlotConfigurations : IEntityTypeConfiguration<ReservedTimeSlot>
{
    public void Configure(EntityTypeBuilder<ReservedTimeSlot> builder)
    {
        ConfigureTable(builder);
    }

    private static void ConfigureTable(EntityTypeBuilder<ReservedTimeSlot> builder)
    {
        builder.ToTable("ReservedTimeSlots");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasOne(x => x.StaffMember)
            .WithMany(x => x.ReservedTimeSlots)
            .HasForeignKey(x => x.StaffMemberId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(x => x.AtDate)
            .IsRequired();

        builder.Property(x => x.CreatedDateTime)
            .IsRequired();

        builder.Property(x => x.UpdatedDateTime)
            .IsRequired();

        builder.OwnsOne(x => x.TimeRange);
    }
}
