using Tor.Application.Abstractions;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Tor.Application.Services.Commands.DeleteService;

internal sealed class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand, Result>
{
    private readonly ITorDbContext _context;

    public DeleteServiceCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        await _context.Services
            .Where(x => x.Id == request.ServiceId)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Ok();
    }
}
