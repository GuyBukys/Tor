using FluentResults;
using MediatR;
using Tor.Domain.TierAggregate.Enums;

namespace Tor.Application.Tiers.Commands.UpdateTier;

public record UpdateTierCommand(Guid BusinessId, TierType? TierType) : IRequest<Result>;
