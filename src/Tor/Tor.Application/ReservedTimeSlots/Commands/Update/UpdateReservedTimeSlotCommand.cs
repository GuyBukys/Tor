using FluentResults;
using MediatR;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.ReservedTimeSlots.Commands.Update;

public record UpdateReservedTimeSlotCommand(
    Guid Id,
    DateOnly AtDate,
    TimeRange TimeRange,
    string? Reason) : IRequest<Result>;