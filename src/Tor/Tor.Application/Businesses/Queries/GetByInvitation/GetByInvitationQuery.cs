using FluentResults;
using MediatR;
using Tor.Domain.BusinessAggregate;

namespace Tor.Application.Businesses.Queries.GetByInvitation;

public record GetByInvitationQuery(string InvitationId) : IRequest<Result<Business>>;
