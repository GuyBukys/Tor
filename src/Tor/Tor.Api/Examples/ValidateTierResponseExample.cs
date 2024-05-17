using Swashbuckle.AspNetCore.Filters;
using Tor.Contracts.Tier;
using Tor.Domain.TierAggregate.Enums;

namespace Tor.Api.Examples;

public class ValidateTierResponseExample : IExamplesProvider<ValidateTierResponse>
{
    public ValidateTierResponse GetExamples()
    {
        return new ValidateTierResponse
        {
            IsValid = true,
            OpenPaywall = false,
            RequiredTier = new()
            {
                Type = TierType.Basic,
                Description = "tier example",
                AppointmentApprovals = true,
                AppointmentReminders = true,
                MaximumStaffMembers = 1,
                FreeTrialDuration = TimeSpan.FromDays(30),
                MessageBlasts = true,
                Payments = true,
                ExternalReference = "Basic",
            }
        };
    }
}
