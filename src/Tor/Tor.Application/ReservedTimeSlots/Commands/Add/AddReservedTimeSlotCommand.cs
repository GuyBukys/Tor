using FluentResults;
using MediatR;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.ReservedTimeSlots.Commands.Add;

public record AddReservedTimeSlotCommand(
    Guid StaffMemberId,
    DateOnly AtDate,
    TimeRange TimeRange,
    string? Reason) : IRequest<Result>;
