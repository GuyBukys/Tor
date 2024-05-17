using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;

namespace Tor.Application.ReservedTimeSlots.Commands.Delete;

internal sealed class DeleteReservedTimeSlotCommandHandler : IRequestHandler<DeleteReservedTimeSlotCommand, Result>
{
    private readonly ITorDbContext _context;

    public DeleteReservedTimeSlotCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteReservedTimeSlotCommand request, CancellationToken cancellationToken)
    {
        await _context.ReservedTimeSlots
            .Where(x => x.Id == request.Id)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Ok();
    }
}
