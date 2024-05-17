using FluentResults;
using MediatR;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.Services.Commands.AddService;

public record AddServiceCommand(
    Guid StaffMemberId,
    string Name,
    string Description,
    AmountDetails Amount,
    List<Duration> Durations) : IRequest<Result<Guid>>;
