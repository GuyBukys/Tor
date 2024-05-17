using Swashbuckle.AspNetCore.Filters;
using Tor.Application.Appointments.Commands.ScheduleAppointment;
using Tor.Domain.AppointmentAggregate.Enums;
using Tor.Domain.AppointmentAggregate.ValueObjects;
using Tor.Domain.BusinessAggregate.Enums;

namespace Tor.Api.Examples;

public class ScheduleAppointmentExample : IExamplesProvider<ScheduleAppointmentCommand>
{
    public ScheduleAppointmentCommand GetExamples()
    {
        return new ScheduleAppointmentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            AppointmentType.Regular,
            DateTimeOffset.Now.AddDays(1),
            new ClientDetails("guy", new("+972", "522420554")),
            Guid.NewGuid(),
            new ServiceDetails(
                "guy",
                "description",
                new(100, "ILS"),
                [new(1, 60, DurationType.Work)]),
            "must wash your hair first",
            true);
    }
}
