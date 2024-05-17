using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.TierAggregate;

namespace Tor.Application.Tiers.Commands.UpdateTier;

internal sealed class UpdateTierCommandHandler : IRequestHandler<UpdateTierCommand, Result>
{
    private readonly ITorDbContext _context;

    public UpdateTierCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateTierCommand request, CancellationToken cancellationToken)
    {
        var businessDetails = await _context.Businesses
            .Where(x => x.Id == request.BusinessId)
            .Select(x => new
            {
                x.Id,
                x.CreatedDateTime,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (businessDetails is null)
        {
            return Result.Fail(
                new NotFoundError($"could not find business with id {request.BusinessId}"));
        }

        if (request.TierType is null)
        {
            bool isFreeTrial = DateTime.UtcNow - businessDetails.CreatedDateTime <= Constants.FreeTrialDuration;
            if (isFreeTrial)
            {
                return Result.Ok();
            }

            await _context.Businesses
            .Where(x => x.Id == request.BusinessId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(p => p.TierId, (Guid?)null),
                cancellationToken);
            return Result.Ok();
        }

        Tier tier = await _context.Tiers
            .FirstAsync(x => x.Type == request.TierType, cancellationToken);

        await _context.Businesses
            .Where(x => x.Id == request.BusinessId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(p => p.TierId, tier.Id),
                cancellationToken);

        return Result.Ok();
    }
}
