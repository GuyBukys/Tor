using FluentResults;
using MediatR;

namespace Tor.Application.ReservedTimeSlots.Commands.Delete;

public record DeleteReservedTimeSlotCommand(Guid Id) : IRequest<Result>;
