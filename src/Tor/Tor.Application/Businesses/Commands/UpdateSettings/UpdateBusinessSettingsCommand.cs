using FluentResults;
using MediatR;
using Tor.Domain.BusinessAggregate.ValueObjects;

namespace Tor.Application.Businesses.Commands.UpdateSettings;

public record UpdateBusinessSettingsCommand(
    Guid BusinessId,
    BusinessSettings BusinessSettings) : IRequest<Result>;
