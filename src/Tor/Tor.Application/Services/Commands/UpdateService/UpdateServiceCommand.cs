using FluentResults;
using MediatR;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.Services.Commands.UpdateService;

public record UpdateServiceCommand(
    Guid ServiceId,
    string Name,
    string Description,
    AmountDetails Amount,
    List<Duration> Durations) : IRequest<Result<Service>>;