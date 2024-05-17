using FluentResults;
using MediatR;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.Businesses.Commands.UpdateAddress;

public record UpdateBusinessAddressCommand(
    Guid BusinessId,
    Address Address) : IRequest<Result>;
