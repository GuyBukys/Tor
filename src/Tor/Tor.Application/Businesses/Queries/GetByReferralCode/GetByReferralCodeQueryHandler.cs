using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;

namespace Tor.Application.Businesses.Queries.GetByReferralCode;
internal sealed class GetByReferralCodeQueryHandler : IRequestHandler<GetByReferralCodeQuery, Result<GetByReferralCodeResult>>
{
    private readonly ITorDbContext _context;

    public GetByReferralCodeQueryHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GetByReferralCodeResult>> Handle(GetByReferralCodeQuery request, CancellationToken cancellationToken)
    {
        GetByReferralCodeResult? result = await _context.Businesses
            .Where(x => x.ReferralCode == request.ReferralCode)
            .Select(x => new GetByReferralCodeResult(x.Id, x.Name))
            .FirstOrDefaultAsync(cancellationToken);

        return result is null ?
            Result.Fail<GetByReferralCodeResult>(
                new NotFoundError($"could not find business with referral code {request.ReferralCode}")) :
            result;
    }
}
