using FluentResults;
using MediatR;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.Clients.Commands.UpdatePersonalDetails;

public record UpdateClientPersonalDetailsCommand(
    Guid ClientId,
    string Name,
    string Email,
    DateOnly? BirthDate,
    PhoneNumber PhoneNumber,
    Address? Address) : IRequest<Result>;
