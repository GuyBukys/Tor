using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate;

namespace Tor.Application.Businesses.Commands.UpdateAddress;

internal sealed class UpdateBusinessAddressCommandHandler : IRequestHandler<UpdateBusinessAddressCommand, Result>
{
    private readonly ITorDbContext _context;

    public UpdateBusinessAddressCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateBusinessAddressCommand request, CancellationToken cancellationToken)
    {
        Business? business = await _context.Businesses
            .FirstOrDefaultAsync(x => x.Id == request.BusinessId, cancellationToken);

        if (business is null)
        {
            return Result.Fail(
                new NotFoundError($"could not find Business with id {request.BusinessId}"));
        }

        business.Address = request.Address;
        business.UpdatedDateTime = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
