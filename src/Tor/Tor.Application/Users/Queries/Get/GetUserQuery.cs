using FluentResults;
using MediatR;
using Tor.Domain.UserAggregate;
using Tor.Domain.UserAggregate.Enum;

namespace Tor.Application.Users.Queries.Get;

public record GetUserQuery(
    string UserToken,
    AppType AppType) : IRequest<Result<User>>;
