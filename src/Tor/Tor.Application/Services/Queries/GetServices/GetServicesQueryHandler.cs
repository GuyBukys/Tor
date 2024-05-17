using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate.Entities;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Tor.Application.Services.Queries.GetServices;

internal sealed class GetServicesQueryHandler : IRequestHandler<GetServicesQuery, Result<List<Service>>>
{
    private readonly ITorDbContext _context;

    public GetServicesQueryHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<Service>>> Handle(GetServicesQuery request, CancellationToken cancellationToken)
    {
        bool isStaffMemberExists = await _context.StaffMembers
            .AsNoTracking()
            .AnyAsync(x => x.Id == request.StaffMemberId, cancellationToken);

        if (!isStaffMemberExists)
        {
            return Result.Fail<List<Service>>(
                new NotFoundError($"could not find staff member with id {request.StaffMemberId}"));
        }

        List<Service> services = await _context.Services
            .AsNoTracking()
            .Where(x => x.StaffMemberId == request.StaffMemberId)
            .ToListAsync(cancellationToken);

        return services;
    }
}
