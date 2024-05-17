using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate.Entities;

namespace Tor.Application.ReservedTimeSlots.Commands.Add;

internal sealed class AddReservedTimeSlotCommandHandler : IRequestHandler<AddReservedTimeSlotCommand, Result>
{
    private readonly ITorDbContext _context;

    public AddReservedTimeSlotCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(AddReservedTimeSlotCommand request, CancellationToken cancellationToken)
    {
        bool isStaffMemberExists = await _context.StaffMembers
            .AnyAsync(x => x.Id == request.StaffMemberId, cancellationToken);

        if (!isStaffMemberExists)
        {
            return Result.Fail(
                new NotFoundError($"could not find staff member with id {request.StaffMemberId}"));
        }

        ReservedTimeSlot reservedTimeSlot = new(Guid.NewGuid())
        {
            StaffMemberId = request.StaffMemberId,
            AtDate = request.AtDate,
            TimeRange = request.TimeRange,
            Reason = request.Reason,
            CreatedDateTime = DateTime.UtcNow,
            UpdatedDateTime = DateTime.UtcNow,
        };

        await _context.ReservedTimeSlots.AddAsync(reservedTimeSlot, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
