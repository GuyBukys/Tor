using Bogus;
using Tor.Application.Businesses.Commands.Create;
using Tor.Application.Businesses.Commands.SetDefaultImage;
using Tor.Application.Businesses.Commands.UpdateAddress;
using Tor.Application.Businesses.Commands.UpdateHomepageNote;
using Tor.Application.Businesses.Commands.UpdateImages;
using Tor.Application.Businesses.Commands.UpdatePersonalDetails;
using Tor.Application.Businesses.Commands.UpdateSettings;
using Tor.Application.Businesses.Commands.UpdateWeeklySchedule;
using Tor.Application.Businesses.Notifications.BusinessCreated;
using Tor.Application.Businesses.Queries.GetById;
using Tor.Application.Businesses.Queries.GetByInvitation;
using Tor.Application.Businesses.Queries.GetByStaffMember;
using Tor.Application.Businesses.Queries.IsBusinessExists;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.BusinessAggregate.Enums;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.CategoryAggregate.Enums;
using Tor.Domain.TierAggregate;
using Tor.Domain.UserAggregate.ValueObjects;

namespace Tor.TestsInfra.CustomFakers;

public partial class Fakers
{
    public static class Businesses
    {
        //Domain
        public static readonly Faker<Tier> TierFaker = new Faker<Tier>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.FreeTrialDuration, f => f.Date.Timespan(TimeSpan.FromDays(100)))
            .RuleFor(x => x.MaximumStaffMembers, _ => 5)
            .RuleFor(x => x.ExternalReference, f => f.Lorem.Word());

        public static readonly Faker<BusinessSettings> BusinessSettingsFaker = new RecordFaker<BusinessSettings>()
            .RuleFor(x => x.BookingMinimumTimeInAdvanceInMinutes, _ => 60)
            .RuleFor(x => x.BookingMaximumTimeInAdvanceInDays, 14)
            .RuleFor(x => x.CancelAppointmentMinimumTimeInMinutes, _ => 60)
            .RuleFor(x => x.RescheduleAppointmentMinimumTimeInMinutes, _ => 60)
            .RuleFor(x => x.MaximumAppointmentsForClient, _ => 3)
            .RuleFor(x => x.AppointmentReminderTimeInHours, _ => 3);

        public static readonly Faker<Business> BusinessFaker = new Faker<Business>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.ReferralCode, _ => Guid.NewGuid().ToString())
            .RuleFor(x => x.Categories, f => f.Make(1, () => Categories.CategoryFaker.Generate()))
            .RuleFor(x => x.CreatedDateTime, _ => DateTime.UtcNow)
            .RuleFor(x => x.UpdatedDateTime, _ => DateTime.UtcNow)
            .RuleFor(x => x.IsActive, _ => true)
            .RuleFor(x => x.Name, f => f.Company.CompanyName())
            .RuleFor(x => x.Description, f => f.Lorem.Sentence())
            .RuleFor(x => x.Email, f => f.Person.Email)
            .RuleFor(x => x.InvitationId, _ => Ulid.NewUlid().ToString())
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.Name, f => f.Company.CompanyName())
            .RuleFor(x => x.Cover, _ => Images.ImageFaker.Generate())
            .RuleFor(x => x.Logo, _ => Images.ImageFaker.Generate())
            .RuleFor(x => x.HomepageNote, f => f.Lorem.Sentence())
            .RuleFor(x => x.Settings, _ => BusinessSettingsFaker.Generate())
            .RuleFor(x => x.Description, f => f.Lorem.Sentence())
            .RuleFor(x => x.CreatedDateTime, _ => DateTime.UtcNow)
            .RuleFor(x => x.UpdatedDateTime, _ => DateTime.UtcNow)
            .RuleFor(x => x.PhoneNumbers, f => f.Make(2, () => Common.PhoneNumberFaker.Generate()))
            .RuleFor(x => x.Address, _ => Common.AddressFaker.Generate())
            .RuleFor(x => x.Portfolio, f => f.Make(f.Random.Number(1, 5), () => Images.ImageFaker.Generate()))
            .RuleFor(x => x.Locations, f => f.Make(1, () => new Location(f.PickRandom<LocationType>())))
            .RuleFor(x => x.NotesForAppointment, f => f.Lorem.Sentence())
            .RuleFor(x => x.Tier, _ => TierFaker.Generate());

        //Commands
        public static readonly Faker<SetBusinessDefaultImageCommand> SetBusinessDefaultImageCommandFaker = new RecordFaker<SetBusinessDefaultImageCommand>()
            .RuleFor(x => x.BusinessId, _ => Guid.NewGuid())
            .RuleFor(x => x.SetLogo, _ => false)
            .RuleFor(x => x.SetLogo, _ => false);

        public static readonly Faker<UpdateBusinessImagesCommand> UpdateBusinessImagesCommandFaker = new RecordFaker<UpdateBusinessImagesCommand>()
            .RuleFor(x => x.BusinessId, _ => Guid.NewGuid())
            .RuleFor(x => x.Logo, _ => Images.ImageFaker.Generate())
            .RuleFor(x => x.Cover, _ => Images.ImageFaker.Generate())
            .RuleFor(x => x.Portfolio, f => f.Make(1, () => Images.ImageFaker.Generate()));

        public static readonly Faker<UpdateHomepageNoteCommand> UpdateHomepageNoteCommandFaker = new RecordFaker<UpdateHomepageNoteCommand>()
            .RuleFor(x => x.BusinessId, _ => Guid.NewGuid())
            .RuleFor(x => x.HomepageNote, f => f.Lorem.Sentence());

        public static readonly Faker<UpdateBusinessSettingsCommand> UpdateBusinessSettingsCommandFaker = new RecordFaker<UpdateBusinessSettingsCommand>()
            .RuleFor(x => x.BusinessId, _ => Guid.NewGuid())
            .RuleFor(x => x.BusinessSettings, _ => BusinessSettingsFaker.Generate());

        public static readonly Faker<UpdateBusinessAddressCommand> UpdateBusinessAddressCommandFaker = new RecordFaker<UpdateBusinessAddressCommand>()
            .RuleFor(x => x.BusinessId, _ => Guid.NewGuid())
            .RuleFor(x => x.Address, _ => Common.AddressFaker.Generate());

        public static readonly Faker<UpdateBusinessPersonalDetailsCommand> UpdateBusinessPersonalDetailsCommandFaker = new RecordFaker<UpdateBusinessPersonalDetailsCommand>()
            .RuleFor(x => x.BusinessId, _ => Guid.NewGuid())
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.Description, f => f.Lorem.Sentence())
            .RuleFor(x => x.Email, f => f.Person.Email)
            .RuleFor(x => x.PhoneNumbers, f => f.Make(2, () => Common.PhoneNumberFaker.Generate()))
            .RuleFor(x => x.SocialMedias, f => f.Make(2, () => Common.SocialMediaFaker.Generate()));

        public static readonly Faker<BusinessCreatedNotification> BusinessCreatedNotificationFaker = new RecordFaker<BusinessCreatedNotification>()
            .RuleFor(x => x.BusinessId, _ => Guid.NewGuid())
            .RuleFor(x => x.BusinessName, f => f.Company.CompanyName())
            .RuleFor(x => x.Devices, f => f.Make(2, () => new Device(Guid.NewGuid().ToString())));

        public static readonly Faker<Duration> DurationFaker = new RecordFaker<Duration>()
            .RuleFor(x => x.Order, f => (short)(f.IndexFaker + 1))
            .RuleFor(x => x.ValueInMinutes, f => f.Random.Number(10, 120))
            .RuleFor(x => x.Type, f => f.PickRandom<DurationType>());

        public static readonly Faker<ServiceCommand> ServiceCommandFaker = new RecordFaker<ServiceCommand>()
            .RuleFor(x => x.Name, f => f.PickRandom<CategoryType>().ToString())
            .RuleFor(x => x.Amount, _ => Common.AmountDetailsFaker.Generate())
            .RuleFor(x => x.Durations, f => f.Make(1, () => DurationFaker.Generate()));

        public static readonly Faker<BusinessOwnerCommand> BusinessOwnerCommandFaker = new RecordFaker<BusinessOwnerCommand>()
            .RuleFor(x => x.UserId, _ => Guid.NewGuid())
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.Email, f => f.Person.Email)
            .RuleFor(x => x.PhoneNumber, _ => Common.PhoneNumberFaker.Generate())
            .RuleFor(x => x.WeeklySchedule, _ => Common.WeeklyScheduleFaker.Generate())
            .RuleFor(x => x.Services, f => f.Make(f.Random.Number(1, 5), _ => ServiceCommandFaker.Generate()))
            .RuleFor(x => x.ProfileImage, _ => Images.ImageFaker.Generate());

        public static readonly Faker<CreateBusinessCommand> CreateBusinessCommandFaker = new RecordFaker<CreateBusinessCommand>()
            .RuleFor(x => x.Name, f => f.Company.CompanyName())
            .RuleFor(x => x.Description, f => f.Lorem.Sentence())
            .RuleFor(x => x.Email, f => f.Person.Email)
            .RuleFor(x => x.CategoryIds, f => f.Make(f.Random.Number(1, 5), _ => Guid.NewGuid()))
            .RuleFor(x => x.BusinessOwner, _ => BusinessOwnerCommandFaker.Generate())
            .RuleFor(x => x.WeeklySchedule, _ => Common.WeeklyScheduleFaker.Generate())
            .RuleFor(x => x.PhoneNumbers, f => f.Make(2, () => Common.PhoneNumberFaker.Generate()))
            .RuleFor(x => x.Address, _ => Common.AddressFaker.Generate())
            .RuleFor(x => x.Cover, _ => Images.ImageFaker.Generate())
            .RuleFor(x => x.Logo, _ => Images.ImageFaker.Generate())
            .RuleFor(x => x.ReferringBusinessId, _ => Guid.NewGuid());

        public static readonly Faker<UpdateBusinessWeeklyScheduleCommand> UpdateBusinessWeeklyScheduleCommandFaker = new RecordFaker<UpdateBusinessWeeklyScheduleCommand>()
            .RuleFor(x => x.BusinessId, _ => Guid.NewGuid())
            .RuleFor(x => x.WeeklySchedule, _ => Common.WeeklyScheduleFaker.Generate());

        //Queries
        public static readonly Faker<GetByStaffMemberQuery> GetByStaffMemberQueryFaker = new RecordFaker<GetByStaffMemberQuery>()
            .RuleFor(x => x.StaffMemberId, _ => Guid.NewGuid());

        public static readonly Faker<GetByIdQuery> GetByIdQueryFaker = new RecordFaker<GetByIdQuery>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid());

        public static readonly Faker<GetByInvitationQuery> GetByInvitationQueryFaker = new RecordFaker<GetByInvitationQuery>()
            .RuleFor(x => x.InvitationId, _ => Guid.NewGuid().ToString());

        public static readonly Faker<IsBusinessExistsQuery> IsBusinessExistsQueryFaker = new RecordFaker<IsBusinessExistsQuery>()
            .RuleFor(x => x.Email, f => f.Person.Email);
    }
}
