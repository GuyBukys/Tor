using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;

namespace Tor.Application.Clients.Queries.GetAppointments;

internal sealed class GetClientAppointmentsQueryHandler : IRequestHandler<GetClientAppointmentsQuery, Result<List<ClientAppointmentResult>>>
{
    private readonly ITorDbContext _context;

    public GetClientAppointmentsQueryHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ClientAppointmentResult>>> Handle(GetClientAppointmentsQuery request, CancellationToken cancellationToken)
    {
        bool isClientExists = await _context.Clients
            .AnyAsync(x => x.Id == request.ClientId, cancellationToken);

        if (!isClientExists)
        {
            return Result.Fail<List<ClientAppointmentResult>>(
                new NotFoundError($"could not find client with id {request.ClientId}"));
        }

        List<ClientAppointmentResult> clientAppointments = await _context.Appointments
            .AsNoTracking()
            .Include(x => x.StaffMember)
            .ThenInclude(x => x.Business)
            .Where(x => x.ClientId == request.ClientId)
            .Where(x => x.ScheduledFor >= DateTimeOffset.UtcNow)
            .OrderBy(x => x.ScheduledFor)
            .Select(x => new ClientAppointmentResult(
                x.Id,
                x.StaffMember.Name,
                x.Type,
                x.Status,
                x.ScheduledFor,
                x.ServiceDetails,
                new BusinessDetails(
                    x.StaffMember.Business.Id,
                    x.StaffMember.Business.Name,
                    x.StaffMember.Business.Logo,
                    x.StaffMember.Business.Cover,
                    x.StaffMember.Business.Address,
                    x.StaffMember.Business.PhoneNumbers.First()),
                DateTime.UtcNow.AddMinutes(x.StaffMember.Business.Settings.CancelAppointmentMinimumTimeInMinutes) <= x.ScheduledFor))
            .ToListAsync(cancellationToken);

        return clientAppointments;
    }
}
