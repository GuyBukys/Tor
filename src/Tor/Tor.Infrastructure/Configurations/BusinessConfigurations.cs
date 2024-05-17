using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.BusinessAggregate.Entities;

namespace Tor.Infrastructure.Configurations;

public class BusinessConfigurations : IEntityTypeConfiguration<Business>
{
    public void Configure(EntityTypeBuilder<Business> builder)
    {
        ConfigureBusinessesTable(builder);
    }

    private static void ConfigureBusinessesTable(EntityTypeBuilder<Business> builder)
    {
        builder.ToTable("Businesses");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.HomepageNote)
            .IsRequired(false)
            .HasMaxLength(10000);

        builder.Property(x => x.InvitationId)
            .IsRequired();

        builder.HasIndex(x => x.InvitationId)
            .IsUnique();

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.NotesForAppointment)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(x => x.ReferralCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(b => b.ReferringBusiness)
            .WithMany()
            .HasForeignKey(b => b.ReferringBusinessId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Categories)
            .WithMany(x => x.Businesses);

        builder.HasOne(x => x.Tier)
            .WithMany(x => x.Businesses)
            .HasForeignKey(x => x.TierId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.StaffMembers)
            .WithOne(x => x.Business)
            .HasForeignKey(x => x.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Clients)
            .WithMany(x => x.Businesses)
            .UsingEntity<BusinessClient>();

        builder.HasMany(x => x.BlockedClients)
            .WithMany()
            .UsingEntity<BlockedClient>();

        builder.OwnsOne(x => x.Logo);
        builder.OwnsOne(x => x.Cover);
        builder.OwnsMany(x => x.Portfolio, ib =>
        {
            ib.ToTable("Business_Portfolio");
        });

        builder.OwnsOne(x => x.Address);

        builder.OwnsMany(x => x.PhoneNumbers, pb =>
        {
            pb.ToTable("Businesses_PhoneNumbers");

            pb.Property(x => x.Prefix)
            .IsRequired()
            .HasMaxLength(10);

            pb.Property(x => x.Number)
            .IsRequired()
            .HasMaxLength(20);
        });

        builder.OwnsMany(x => x.SocialMedias, smb =>
        {
            smb.ToTable("Businesses_SocialMedias");

            smb.Property(x => x.Type)
            .IsRequired();

            smb.Property(x => x.Url)
            .IsRequired();
        });

        builder.OwnsMany(x => x.Locations, cb =>
        {
            cb.ToTable("Businesses_Locations");

            cb.Property(x => x.Type)
            .IsRequired();
        });

        builder.OwnsOne(x => x.Settings, sb =>
        {
            sb.Property(x => x.BookingMinimumTimeInAdvanceInMinutes)
            .IsRequired()
            .HasDefaultValue(60);

            sb.Property(x => x.BookingMaximumTimeInAdvanceInDays)
            .IsRequired()
            .HasDefaultValue(14);

            sb.Property(x => x.CancelAppointmentMinimumTimeInMinutes)
            .IsRequired()
            .HasDefaultValue(180);

            sb.Property(x => x.RescheduleAppointmentMinimumTimeInMinutes)
            .IsRequired()
            .HasDefaultValue(180);

            sb.Property(x => x.MaximumAppointmentsForClient)
            .IsRequired()
            .HasDefaultValue(3);

            sb.Property(x => x.AppointmentReminderTimeInHours)
            .IsRequired()
            .HasDefaultValue(24);
        });
    }
}
