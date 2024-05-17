using Swashbuckle.AspNetCore.Filters;
using Tor.Contracts.Business;
using Tor.Contracts.Service;
using Tor.Contracts.StaffMember;
using Tor.Domain.BusinessAggregate.Enums;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.Common.ValueObjects;
using Tor.Domain.TierAggregate.Enums;

namespace Tor.Api.Examples;

public class GetBusinessResponseExample : IExamplesProvider<GetBusinessResponse>
{
    public GetBusinessResponse GetExamples()
    {
        return new GetBusinessResponse
        {
            Tier = new()
            {
                AppointmentApprovals = true,
                AppointmentReminders = true,
                ExternalReference = "Basic",
                Description = "test tier",
                FreeTrialDuration = TimeSpan.FromDays(30),
                MaximumStaffMembers = 1,
                MessageBlasts = true,
                Payments = true,
                Type = TierType.Basic,
            },
            Id = Guid.NewGuid(),
            Name = "test",
            Description = "test",
            Email = "test@test.com",
            HomepageNote = "this is a homepage note",
            Address = new("Ashkelon", "Hamra 12", 34, 100, 100, "next exit to the right"),
            Settings = new(180, 14, 180, 180, 3, 24),
            Cover = CommonExamples.Image,
            Logo = CommonExamples.Image,
            Portfolio = [CommonExamples.Image,],
            WeeklySchedule = CommonExamples.WeeklySchedule,
            PhoneNumbers =
            [
                new PhoneNumber("+972", "522420554"),
            ],
            SocialMedias =
            [
                new SocialMedia(SocialMediaType.Instagram, "https://google.com"),
            ],
            StaffMembers =
            [
                new GetStaffMemberResponse
                {
                    Id = Guid.NewGuid(),
                    Name = "test staff member",
                    Description = "this is a staff member description",
                    Email = "test@test.com",
                    Address = new("Ashkelon", "Hamra 12", 34, 100, 100, "next exit to the right"),
                    BirthDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    PhoneNumber = new("+972", "522420554"),
                    Position = PositionType.BusinessOwner,
                    ProfileImage = CommonExamples.Image,
                    Services =
                    [
                        new ServiceResponse
                        {
                            Id = Guid.NewGuid(),
                            Name = "test service",
                            Description = "this is a test service",
                            Amount = new(100, "ILS"),
                            Durations =
                            [
                                new Duration(1, 60, DurationType.Work),
                                new Duration(2, 10, DurationType.Gap),
                                new Duration(3, 30, DurationType.Work),
                            ],
                        },
                    ],
                    WeeklySchedule = CommonExamples.WeeklySchedule,
                },
            ],
        };
    }
}
