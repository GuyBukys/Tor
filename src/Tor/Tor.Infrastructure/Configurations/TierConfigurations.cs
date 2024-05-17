using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tor.Domain.TierAggregate;
using Tor.Domain.TierAggregate.Enums;

namespace Tor.Infrastructure.Configurations;

public class TierConfigurations : IEntityTypeConfiguration<Tier>
{
    public void Configure(EntityTypeBuilder<Tier> builder)
    {
        ConfigureTiersTable(builder);
    }

    private static void ConfigureTiersTable(EntityTypeBuilder<Tier> builder)
    {
        builder.ToTable("Tiers");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasMany(x => x.Businesses)
            .WithOne(x => x.Tier)
            .HasForeignKey(x => x.TierId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasData(new List<Tier>()
        {
            new(new Guid("7ca11ea5-626a-4f7c-9677-b3e0cfe6f669"))
            {
                Type = TierType.Basic,
                Description = "basic tier for new businesses",
                Payments = false,
                MaximumStaffMembers = 1,
                AppointmentApprovals = false,
                AppointmentReminders = false,
                MessageBlasts = false,
                ExternalReference = "Basic",
                FreeTrialDuration = TimeSpan.FromDays(30),
            },
            new(new Guid("c6d5d1e0-6081-4382-9d48-f7243b357411"))
            {
                Type = TierType.Premium,
                Description = "premium tier for mid tier businesses",
                Payments = false,
                MaximumStaffMembers = 3,
                AppointmentApprovals = true,
                AppointmentReminders = true,
                MessageBlasts = true,
                ExternalReference = "Premium",
                FreeTrialDuration = TimeSpan.Zero,
            },
            new(new Guid("8c8c5bf9-9fbf-4fc9-8df0-f3505aa8798e"))
            {
                Type = TierType.Enterprise,
                Description = "enterprise tier for payments",
                Payments = true,
                MaximumStaffMembers = 10,
                AppointmentApprovals = true,
                AppointmentReminders = true,
                MessageBlasts = true,
                ExternalReference = "Enterprise",
                FreeTrialDuration = TimeSpan.Zero,
            },
        });
    }
}
