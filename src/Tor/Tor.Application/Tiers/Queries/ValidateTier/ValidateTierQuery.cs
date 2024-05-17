using FluentResults;
using MediatR;

namespace Tor.Application.Tiers.Queries.ValidateTier;

public record ValidateTierQuery(Guid StaffMemberId, string? ExternalReference) : IRequest<Result<ValidateTierResult>>;
