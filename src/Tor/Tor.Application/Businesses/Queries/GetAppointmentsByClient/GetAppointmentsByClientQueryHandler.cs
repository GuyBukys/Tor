using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.AppointmentAggregate;

namespace Tor.Application.Businesses.Queries.GetAppointmentsByClient;

internal sealed class GetAppointmentsByClientQueryHandler : IRequestHandler<GetAppointmentsByClientQuery, Result<List<Appointment>>>
{
    private readonly ITorDbContext _context;

    public GetAppointmentsByClientQueryHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<Appointment>>> Handle(GetAppointmentsByClientQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Guid>? staffMemberIds = await _context.Businesses
            .Where(x => x.Id == request.BusinessId)
            .Select(x => x.StaffMembers.Select(s => s.Id))
            .FirstOrDefaultAsync(cancellationToken);
        if (staffMemberIds is null)
        {
            return Result.Fail<List<Appointment>>(
                new NotFoundError($"could not find business with id {request.BusinessId}"));
        }

        bool isClientExists = await _context.Clients
            .AnyAsync(x => x.Id == request.ClientId, cancellationToken);
        if (!isClientExists)
        {
            return Result.Fail<List<Appointment>>(
                new NotFoundError($"could not find client with id {request.ClientId}"));
        }

        return await _context.Appointments
            .AsNoTracking()
            .Include(x => x.StaffMember)
            .Where(x => x.ClientId == request.ClientId)
            .Where(x => staffMemberIds.Contains(x.StaffMemberId))
            .ToListAsync(cancellationToken);
    }
}
