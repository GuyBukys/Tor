using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate.Entities;

namespace Tor.Application.ReservedTimeSlots.Commands.Update;

internal sealed class UpdateReservedTimeSlotCommandHandler : IRequestHandler<UpdateReservedTimeSlotCommand, Result>
{
    private readonly ITorDbContext _context;

    public UpdateReservedTimeSlotCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateReservedTimeSlotCommand request, CancellationToken cancellationToken)
    {
        ReservedTimeSlot? reservedTimeSlot = await _context.ReservedTimeSlots
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (reservedTimeSlot is null)
        {
            return Result.Fail(
                new NotFoundError($"could not find reserved time slot with id {request.Id}"));
        }

        reservedTimeSlot.AtDate = request.AtDate;
        reservedTimeSlot.TimeRange = request.TimeRange;
        reservedTimeSlot.Reason = request.Reason;
        reservedTimeSlot.UpdatedDateTime = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
