using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;

namespace Tor.Application.Businesses.Queries.CanAddStaffMember;

internal sealed class CanAddStaffMemberQueryHandler : IRequestHandler<CanAddStaffMemberQuery, Result<bool>>
{
    private readonly ITorDbContext _context;

    public CanAddStaffMemberQueryHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(CanAddStaffMemberQuery request, CancellationToken cancellationToken)
    {
        var businessDetails = await _context.Businesses
            .AsNoTracking()
            .Where(x => x.Id == request.BusinessId)
            .Select(x => new
            {
                x.CreatedDateTime,
                x.Tier,
                StaffMemberCount = x.StaffMembers.Count(),
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (businessDetails is null)
        {
            return Result.Fail<bool>(
                new NotFoundError($"could not find business with id {request.BusinessId}"));
        }

        bool isStaffMemberCountGreaterThanTier = businessDetails.StaffMemberCount < businessDetails.Tier.MaximumStaffMembers;

        return isStaffMemberCountGreaterThanTier;
    }
}
