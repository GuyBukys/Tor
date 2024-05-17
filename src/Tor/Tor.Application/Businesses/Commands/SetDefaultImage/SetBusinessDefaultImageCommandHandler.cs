using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.Common.Enums;

namespace Tor.Application.Businesses.Commands.SetDefaultImage;

internal sealed class SetBusinessDefaultImageCommandHandler : IRequestHandler<SetBusinessDefaultImageCommand, Result>
{
    private readonly ITorDbContext _context;
    private readonly IStorageManager _storageManager;

    public SetBusinessDefaultImageCommandHandler(ITorDbContext context, IStorageManager storageManager)
    {
        _context = context;
        _storageManager = storageManager;
    }

    public async Task<Result> Handle(SetBusinessDefaultImageCommand request, CancellationToken cancellationToken)
    {
        Business? business = await _context.Businesses
            .Include(x => x.Categories)
            .FirstOrDefaultAsync(x => x.Id == request.BusinessId, cancellationToken);

        if (business is null)
        {
            return Result.Fail(
                new NotFoundError($"could not find Business with id {request.BusinessId}"));
        }

        if (request.SetLogo)
        {
            business.Logo = _storageManager.GetDefaultImage(ImageType.Logo, EntityType.Business);
        }

        if (request.SetCover)
        {
            business.Cover = _storageManager.GetDefaultImage(ImageType.Cover, EntityType.Business, business.Categories.First().Type);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
