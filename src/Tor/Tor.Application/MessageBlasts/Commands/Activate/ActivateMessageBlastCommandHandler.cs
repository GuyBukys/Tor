using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.MessageBlastAggregate.Entities;

namespace Tor.Application.MessageBlasts.Commands.Activate;

internal sealed class ActivateMessageBlastCommandHandler : IRequestHandler<ActivateMessageBlastCommand, Result>
{
    private readonly ITorDbContext _context;

    public ActivateMessageBlastCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(ActivateMessageBlastCommand request, CancellationToken cancellationToken)
    {
        BusinessMessageBlast? businessMessageBlast = await _context.BusinessMessageBlasts
            .Where(x => x.BusinessId == request.BusinessId)
            .Where(x => x.MessageBlastId == request.MessageBlastId)
            .FirstOrDefaultAsync(cancellationToken);

        if (businessMessageBlast is null)
        {
            return Result.Fail(
                new NotFoundError($"could not find message blast for busines {request.BusinessId} and message blast id {request.MessageBlastId}"));
        }

        businessMessageBlast.IsActive = true;
        businessMessageBlast.Body = request.Body;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
