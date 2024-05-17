using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate.Enums;
using Tor.Domain.TierAggregate;
using Tor.Domain.TierAggregate.Enums;


namespace Tor.Application.Tiers.Queries.ValidateTier;

internal sealed class ValidateTierQueryHandler : IRequestHandler<ValidateTierQuery, Result<ValidateTierResult>>
{
    private readonly ITorDbContext _context;

    public ValidateTierQueryHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ValidateTierResult>> Handle(ValidateTierQuery request, CancellationToken cancellationToken)
    {
        var businessDetails = await _context.StaffMembers
            .AsNoTracking()
            .Where(x => x.Id == request.StaffMemberId)
            .Select(x => new BusinessDetails
            {
                StaffMemberCount = x.Business.StaffMembers.Count,
                StaffMemberPosition = x.Position,
                CreatedDateTime = x.Business.CreatedDateTime,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (businessDetails is null)
        {
            return Result.Fail<ValidateTierResult>(
                new NotFoundError($"could not find business for staff member with id {request.StaffMemberId}"));
        }

        if (string.IsNullOrEmpty(request.ExternalReference))
        {
            return await ValidateBusinessWithoutTier(businessDetails, cancellationToken);
        }

        Tier? tier = await _context.Tiers.FirstOrDefaultAsync(x => x.ExternalReference == request.ExternalReference, cancellationToken);
        if (tier is null)
        {
            return Result.Fail<ValidateTierResult>(
                new NotFoundError($"could not find tier with external reference: {request.ExternalReference}"));
        }

        return await ValidateBusinessWithTier(businessDetails, tier, cancellationToken);
    }

    private async Task<ValidateTierResult> ValidateBusinessWithoutTier(BusinessDetails businessDetails, CancellationToken cancellationToken)
    {
        bool isFreeTrial = DateTime.UtcNow - businessDetails.CreatedDateTime <= Constants.FreeTrialDuration;
        if (isFreeTrial)
        {
            Tier premiumTier = await _context.Tiers.FirstAsync(x => x.Type == TierType.Premium, cancellationToken);
            return new ValidateTierResult(true, false, premiumTier);
        }

        Tier requiredTier = await GetTierByStaffMemberCount(businessDetails.StaffMemberCount, cancellationToken);
        bool openPaywall = businessDetails.StaffMemberPosition == PositionType.BusinessOwner;

        return new ValidateTierResult(false, openPaywall, requiredTier);
    }

    private async Task<ValidateTierResult> ValidateBusinessWithTier(BusinessDetails businessDetails, Tier existingTier, CancellationToken cancellationToken)
    {
        if (businessDetails.StaffMemberCount <= existingTier.MaximumStaffMembers)
        {
            return new ValidateTierResult(true, false, null);
        }

        Tier requiredTier = await GetTierByStaffMemberCount(businessDetails.StaffMemberCount, cancellationToken);
        bool openPaywall = businessDetails.StaffMemberPosition == PositionType.BusinessOwner;

        return new ValidateTierResult(
            false,
            openPaywall,
            requiredTier);
    }

    private async Task<Tier> GetTierByStaffMemberCount(int staffMemberCount, CancellationToken cancellationToken)
    {
        TierType requiredTierType = staffMemberCount switch
        {
            1 => TierType.Basic,
            > 1 and <= 3 => TierType.Premium,
            > 3 => TierType.Enterprise,
            _ => throw new UnreachableException("cannot find staff member cound for required tier type"),
        };

        return await _context.Tiers.FirstAsync(x => x.Type == requiredTierType, cancellationToken);
    }
    private class BusinessDetails
    {
        public int StaffMemberCount { get; set; }
        public PositionType StaffMemberPosition { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
