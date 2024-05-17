using Tor.Application.Abstractions.Models;
using Tor.Domain.AppointmentAggregate;

namespace Tor.Application.Abstractions;

public interface IAppointmentRepository
{
    Task<Appointment> Create(CreateAppointmentInput input, CancellationToken cancellationToken);
    Task Cancel(Guid appointmentId, CancellationToken cancellationToken);
}