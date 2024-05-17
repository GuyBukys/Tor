using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate.Entities;

namespace Tor.Application.StaffMembers.Queries.GetById;

internal sealed class GetStaffMemberByIdQueryHandler : IRequestHandler<GetStaffMemberByIdQuery, Result<StaffMember>>
{
    private readonly ITorDbContext _context;

    public GetStaffMemberByIdQueryHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result<StaffMember>> Handle(GetStaffMemberByIdQuery request, CancellationToken cancellationToken)
    {
        StaffMember? staffMember = await _context.StaffMembers
            .AsNoTracking()
            .Include(x => x.Services)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (staffMember is null)
        {
            return Result.Fail<StaffMember>(
                new NotFoundError($"could not find staff member with id {request.Id}"));
        }

        return staffMember!;
    }
}
