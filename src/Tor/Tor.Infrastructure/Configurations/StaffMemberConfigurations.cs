using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Infrastructure.Utils;

namespace Tor.Infrastructure.Configurations;

public class StaffMemberConfigurations : IEntityTypeConfiguration<StaffMember>
{
    public void Configure(EntityTypeBuilder<StaffMember> builder)
    {
        ConfigureStaffMembersTable(builder);
    }

    private static void ConfigureStaffMembersTable(EntityTypeBuilder<StaffMember> builder)
    {
        builder.ToTable("StaffMembers");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasOne(x => x.Business)
            .WithMany(x => x.StaffMembers)
            .HasForeignKey(x => x.BusinessId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.WaitingLists)
            .WithOne(x => x.StaffMember)
            .HasForeignKey(x => x.StaffMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey((StaffMember s) => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(x => x.ProfileImage);

        builder.HasMany(x => x.Services)
            .WithOne(x => x.StaffMember)
            .HasForeignKey(x => x.StaffMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Appointments)
            .WithOne(x => x.StaffMember)
            .HasForeignKey(x => x.StaffMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.BirthDate)
            .IsRequired(false)
            .HasConversion<DateOnlyConverter, DateOnlyComparer>();

        builder.Property(x => x.Position)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired(false)
            .HasMaxLength(10000);

        builder.OwnsOne(x => x.Address);

        builder.OwnsOne(x => x.PhoneNumber);

        builder.HasMany(x => x.ReservedTimeSlots)
            .WithOne(x => x.StaffMember)
            .HasForeignKey(x => x.StaffMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsMany(x => x.CustomWorkingDays, cb =>
        {
            cb.ToTable("StaffMembers_CustomWorkingDays");
            cb.Property(x => x.AtDate)
                .IsRequired();

            cb.OwnsOne(x => x.DailySchedule, db =>
            {
                db.OwnsOne(x => x.TimeRange);
                db.OwnsMany(x => x.RecurringBreaks, rb =>
                {
                    rb.ToTable("StaffMembers_CustomWorkingDay_RecurringBreaks");
                    rb.OwnsOne(x => x.TimeRange);
                });
            });
        });

        builder.OwnsOne(x => x.Settings, sb =>
        {
            sb.Property(x => x.SendNotificationsWhenAppointmentScheduled)
                .IsRequired()
                .HasDefaultValue(true);

            sb.Property(x => x.SendNotificationsWhenAppointmentCanceled)
                .IsRequired()
                .HasDefaultValue(true);
        });

        builder.OwnsOne(x => x.WeeklySchedule, wsb =>
        {
            wsb.ToJson();
            wsb.OwnsOne(x => x.Sunday, sb =>
            {
                sb.OwnsOne(x => x.TimeRange);
                sb.OwnsMany(x => x.RecurringBreaks, rbb =>
                {
                    rbb.OwnsOne(x => x.TimeRange);
                });
            });
            wsb.OwnsOne(x => x.Monday, sb =>
            {
                sb.OwnsOne(x => x.TimeRange);
                sb.OwnsMany(x => x.RecurringBreaks, rbb =>
                {
                    rbb.OwnsOne(x => x.TimeRange);
                });
            });
            wsb.OwnsOne(x => x.Tuesday, sb =>
            {
                sb.OwnsOne(x => x.TimeRange);
                sb.OwnsMany(x => x.RecurringBreaks, rbb =>
                {
                    rbb.OwnsOne(x => x.TimeRange);
                });
            });
            wsb.OwnsOne(x => x.Wednesday, sb =>
            {
                sb.OwnsOne(x => x.TimeRange);
                sb.OwnsMany(x => x.RecurringBreaks, rbb =>
                {
                    rbb.OwnsOne(x => x.TimeRange);
                });
            });
            wsb.OwnsOne(x => x.Thursday, sb =>
            {
                sb.OwnsOne(x => x.TimeRange);
                sb.OwnsMany(x => x.RecurringBreaks, rbb =>
                {
                    rbb.OwnsOne(x => x.TimeRange);
                });
            });
            wsb.OwnsOne(x => x.Friday, sb =>
            {
                sb.OwnsOne(x => x.TimeRange);
                sb.OwnsMany(x => x.RecurringBreaks, rbb =>
                {
                    rbb.OwnsOne(x => x.TimeRange);
                });
            });
            wsb.OwnsOne(x => x.Saturday, sb =>
            {
                sb.OwnsOne(x => x.TimeRange);
                sb.OwnsMany(x => x.RecurringBreaks, rbb =>
                {
                    rbb.OwnsOne(x => x.TimeRange);
                });
            });
        });
    }
}
