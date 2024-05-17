using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate;

namespace Tor.Application.Businesses.Commands.UpdateSettings;

internal sealed class UpdateBusinessSettingsCommandHandler : IRequestHandler<UpdateBusinessSettingsCommand, Result>
{
    private readonly ITorDbContext _context;

    public UpdateBusinessSettingsCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateBusinessSettingsCommand request, CancellationToken cancellationToken)
    {
        Business? business = await _context.Businesses
            .Where(x => x.Id == request.BusinessId)
            .FirstOrDefaultAsync(cancellationToken);

        if (business is null)
        {
            return Result.Fail(
                new NotFoundError($"could not find business with id {request.BusinessId}"));
        }

        business.Settings = request.BusinessSettings;
        business.UpdatedDateTime = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
