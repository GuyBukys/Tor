using Bogus;
using Tor.Application.Appointments.Commands.CancelAppointment;
using Tor.Application.Appointments.Commands.RescheduleAppointment;
using Tor.Application.Appointments.Commands.ScheduleAppointment;
using Tor.Application.Appointments.Notifications.AppointmentCanceled;
using Tor.Domain.AppointmentAggregate;
using Tor.Domain.AppointmentAggregate.Enums;
using Tor.Domain.AppointmentAggregate.ValueObjects;

namespace Tor.TestsInfra.CustomFakers;

public partial class Fakers
{
    public static class Appointments
    {
        public static readonly Faker<ClientDetails> ClientDetailsFaker = new RecordFaker<ClientDetails>()
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.PhoneNumber, _ => Common.PhoneNumberFaker.Generate());

        public static readonly Faker<ServiceDetails> ServiceDetailsFaker = new RecordFaker<ServiceDetails>()
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.Description, f => f.Lorem.Sentence())
            .RuleFor(x => x.Amount, _ => Common.AmountDetailsFaker.Generate())
            .RuleFor(x => x.Durations, f => f.Make(1, () => Businesses.DurationFaker.Generate()));


        public static readonly Faker<Appointment> AppointmentFaker = new Faker<Appointment>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.StaffMemberId, _ => Guid.NewGuid())
            .RuleFor(x => x.ClientId, _ => Guid.NewGuid())
            .RuleFor(x => x.ScheduledFor, f => DateTimeOffset.UtcNow)
            .RuleFor(x => x.Status, _ => AppointmentStatusType.Created)
            .RuleFor(x => x.ClientDetails, _ => ClientDetailsFaker.Generate())
            .RuleFor(x => x.ServiceDetails, _ => ServiceDetailsFaker.Generate())
            .RuleFor(x => x.Notes, f => f.Lorem.Sentence())
            .RuleFor(x => x.HasReceivedReminder, _ => false);

        public static readonly Faker<ScheduleAppointmentCommand> ScheduleAppointmentCommandFaker = new RecordFaker<ScheduleAppointmentCommand>()
            .RuleFor(x => x.StaffMemberId, _ => Guid.NewGuid())
            .RuleFor(x => x.ClientId, _ => null)
            .RuleFor(x => x.Type, f => f.PickRandom<AppointmentType>())
            .RuleFor(x => x.ScheduledFor, _ => DateTime.UtcNow.AddDays(1))
            .RuleFor(x => x.ServiceDetails, _ => ServiceDetailsFaker.Generate())
            .RuleFor(x => x.ClientDetails, _ => ClientDetailsFaker.Generate());

        public static readonly Faker<RescheduleAppointmentCommand> RescheduleAppointmentCommandFaker = new RecordFaker<RescheduleAppointmentCommand>()
            .RuleFor(x => x.AppointmentId, _ => Guid.NewGuid())
            .RuleFor(x => x.ScheduledFor, _ => DateTime.UtcNow.AddDays(1))
            .RuleFor(x => x.ClientDetails, _ => ClientDetailsFaker.Generate())
            .RuleFor(x => x.ServiceDetails, _ => ServiceDetailsFaker.Generate())
            .RuleFor(x => x.Notes, f => f.Lorem.Sentence())
            .RuleFor(x => x.ScheduledFor, _ => DateTime.UtcNow.AddDays(1));

        public static readonly Faker<CancelAppointmentCommand> CancelAppointmentCommandFaker = new RecordFaker<CancelAppointmentCommand>()
            .RuleFor(x => x.AppointmentId, _ => Guid.NewGuid())
            .RuleFor(x => x.ForceCancel, _ => false);

        public static readonly Faker<AppointmentCanceledNotification> AppointmentCanceledNotificationFaker = new RecordFaker<AppointmentCanceledNotification>()
            .RuleFor(x => x.ClientId, _ => null!)
            .RuleFor(x => x.AppointmentScheduledDate, _ => DateTimeOffset.UtcNow)
            .RuleFor(x => x.NotifyClient, _ => false)
            .RuleFor(x => x.NotifyWaitingList, _ => false)
            .RuleFor(x => x.Reason, f => f.Lorem.Sentence());
    }
}
