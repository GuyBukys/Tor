using FluentResults;
using MediatR;
using Tor.Domain.ClientAggregate;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.Clients.Commands.Create;

public record CreateClientCommand(
    Guid UserId,
    string Name,
    string Email,
    DateOnly? BirthDate,
    PhoneNumber PhoneNumber,
    Address? Address,
    Image? ProfileImage) : IRequest<Result<Client>>;
