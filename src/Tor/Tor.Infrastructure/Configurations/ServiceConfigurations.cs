using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tor.Domain.AppointmentAggregate;
using Tor.Domain.BusinessAggregate.Entities;

namespace Tor.Infrastructure.Configurations;

public class ServiceConfigurations : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        ConfigureServicesTable(builder);
    }

    private static void ConfigureServicesTable(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Services");

        builder.HasOne(x => x.StaffMember)
            .WithMany(x => x.Services)
            .HasForeignKey(x => x.StaffMemberId)
            .OnDelete(DeleteBehavior.NoAction);


        builder.HasMany<Appointment>()
            .WithOne(x => x.Service)
            .HasForeignKey(x => x.ServiceId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.OwnsOne(x => x.Amount);

        builder.OwnsMany(x => x.Durations, db =>
        {
            db.ToTable("Services_Durations");
        });
    }
}
