using FluentResults;
using MediatR;
using Tor.Domain.Common.ValueObjects;
using Tor.Domain.UserAggregate;
using Tor.Domain.UserAggregate.Enum;

namespace Tor.Application.Users.Commands.Create;

public record CreateUserCommand(
    string UserToken,
    string DeviceToken,
    PhoneNumber PhoneNumber,
    AppType AppType) : IRequest<Result<User>>;
