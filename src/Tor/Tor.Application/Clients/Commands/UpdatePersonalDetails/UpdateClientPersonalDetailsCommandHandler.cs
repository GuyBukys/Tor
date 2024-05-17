using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Clients.Commands.UpdatePersonalDetails;
using Tor.Application.Common.Errors.ErrorTypes;

namespace Tor.Application.StaffMembers.Commands.UpdatePersonalDetails;

internal sealed class UpdateClientPersonalDetailsCommandHandler : IRequestHandler<UpdateClientPersonalDetailsCommand, Result>
{
    private readonly ITorDbContext _context;

    public UpdateClientPersonalDetailsCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateClientPersonalDetailsCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients
            .FirstOrDefaultAsync(x => x.Id == request.ClientId, cancellationToken);

        if (client is null)
        {
            return Result.Fail(
                new NotFoundError($"could not find client with id {request.ClientId}"));
        }

        client.Name = request.Name;
        client.Email = request.Email;
        client.BirthDate = request.BirthDate;
        client.PhoneNumber = request.PhoneNumber;
        client.Address = request.Address;
        client.UpdatedDateTime = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
