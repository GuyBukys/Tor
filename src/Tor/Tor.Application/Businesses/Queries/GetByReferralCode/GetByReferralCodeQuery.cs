using FluentResults;
using MediatR;

namespace Tor.Application.Businesses.Queries.GetByReferralCode;

public record GetByReferralCodeQuery(string ReferralCode) : IRequest<Result<GetByReferralCodeResult>>;
