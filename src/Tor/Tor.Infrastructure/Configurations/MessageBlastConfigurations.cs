using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tor.Domain.MessageBlastAggregate;
using Tor.Domain.MessageBlastAggregate.Entities;
using Tor.Domain.MessageBlastAggregate.Enums;
using ApplicationConstants = Tor.Application.Constants;

namespace Tor.Infrastructure.Configurations;

public class MessageBlastConfigurations : IEntityTypeConfiguration<MessageBlast>
{
    private static readonly List<MessageBlast> _defaultMessageBlasts =
        [
            new MessageBlast(new Guid("3e82a3ad-7474-4243-9e45-620206852b43"))
            {
                IsActive = true,
                Name = ApplicationConstants.MessageBlasts.ScheduleAppointmentReminderName,
                Description = "Send a notification to clients who havent booked an appointment in the business for over a month",
                DisplayName = ApplicationConstants.MessageBlasts.ScheduleAppointmentReminderDisplayName,
                DisplayDescription = "שליחת תזכורת ללקוח אשר לא קבע תור בעסק מעל חודש",
                Type = MessageBlastType.Reactivate,
                TemplateBody = "עבר זמן מאז שהיית אצלנו בפעם האחרונה. נשמח לראותך בקרוב!",
                CanEditBody = true,
                CreatedDateTime = new DateTime(2023, 12, 27),
                UpdatedDateTime = new DateTime(2023, 12, 27),
            },
        ];

    public void Configure(EntityTypeBuilder<MessageBlast> builder)
    {
        ConfigureTable(builder);
    }

    private static void ConfigureTable(EntityTypeBuilder<MessageBlast> builder)
    {
        builder.ToTable("MessageBlasts");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.DisplayName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.DisplayDescription)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.TemplateBody)
            .IsRequired(false)
            .HasMaxLength(50000);

        builder.Property(x => x.CreatedDateTime)
            .IsRequired();

        builder.Property(x => x.UpdatedDateTime)
            .IsRequired();

        builder.HasMany(x => x.Businesses)
            .WithMany(x => x.MessageBlasts)
            .UsingEntity<BusinessMessageBlast>(eb =>
            {
                eb.ToTable("BusinessMessageBlasts");

                eb.Property(x => x.IsActive)
                    .IsRequired()
                    .HasDefaultValue(false);

                eb.Property(x => x.Body)
                    .IsRequired(false)
                    .HasMaxLength(50000);
            });

        builder.HasData(_defaultMessageBlasts);
    }
}
