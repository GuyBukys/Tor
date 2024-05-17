using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tor.Domain.AppointmentAggregate;

namespace Tor.Infrastructure.Configurations;

public class AppointmentConfigurations : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        ConfigureAppointmentsTable(builder);
    }

    private static void ConfigureAppointmentsTable(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.HasOne(x => x.Client)
            .WithMany()
            .HasForeignKey(x => x.ClientId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Service)
            .WithMany()
            .HasForeignKey(x => x.ServiceId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.StaffMember)
            .WithMany()
            .HasForeignKey(x => x.StaffMemberId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(x => x.ScheduledFor)
            .IsRequired();

        builder.Property(x => x.HasReceivedReminder)
            .IsRequired()
            .HasDefaultValue(false);

        builder.OwnsOne(x => x.ClientDetails, cdb =>
        {
            cdb.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            cdb.OwnsOne(x => x.PhoneNumber);
        });

        builder.OwnsOne(x => x.ServiceDetails, sdb =>
        {
            sdb.Property(x => x.Description)
            .IsRequired(false);

            sdb.OwnsOne(x => x.Amount);
            sdb.OwnsMany(x => x.Durations);
        });
    }
}
