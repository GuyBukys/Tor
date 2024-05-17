using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Domain.AppointmentAggregate;
using Tor.Domain.AppointmentAggregate.Enums;

namespace Tor.Infrastructure.Persistence.Repositories;

internal sealed class AppointmentRepository : IAppointmentRepository
{
    private readonly TorDbContext _context;

    public AppointmentRepository(TorDbContext context)
    {
        _context = context;
    }

    public async Task<Appointment> Create(CreateAppointmentInput input, CancellationToken cancellationToken)
    {
        AppointmentStatusType status = DateTime.UtcNow.AddHours(input.AppointmentReminderTimeInHours) > input.ScheduledFor ?
            AppointmentStatusType.Approved :
            AppointmentStatusType.Created;

        Appointment appointment = new(Guid.NewGuid())
        {
            StaffMemberId = input.StaffMemberId,
            ClientId = input.ClientId,
            ScheduledFor = input.ScheduledFor,
            ServiceDetails = input.ServiceDetails,
            ClientDetails = input.ClientDetails,
            CreatedDateTime = DateTime.UtcNow,
            UpdatedDateTime = DateTime.UtcNow,
            Type = input.Type,
            Status = status,
            Notes = input.Notes,
            HasReceivedReminder = false,
        };

        await _context.Appointments.AddAsync(appointment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return appointment;
    }

    public async Task Cancel(Guid appointmentId, CancellationToken cancellationToken)
    {
        await _context.Appointments
            .Where(x => x.Id == appointmentId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(p => p.Status, AppointmentStatusType.Canceled)
                .SetProperty(x => x.UpdatedDateTime, DateTime.UtcNow),
                cancellationToken);
    }
}
