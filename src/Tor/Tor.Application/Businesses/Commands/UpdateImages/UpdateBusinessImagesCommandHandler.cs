using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate;

namespace Tor.Application.Businesses.Commands.UpdateImages;

internal sealed class UpdateBusinessImagesCommandHandler : IRequestHandler<UpdateBusinessImagesCommand, Result>
{
    private readonly ITorDbContext _context;
    private readonly IStorageManager _storageManager;

    public UpdateBusinessImagesCommandHandler(
        ITorDbContext context,
        IStorageManager storageManager)
    {
        _context = context;
        _storageManager = storageManager;
    }

    public async Task<Result> Handle(UpdateBusinessImagesCommand request, CancellationToken cancellationToken)
    {
        Business? business = await _context.Businesses
            .FirstOrDefaultAsync(x => x.Id == request.BusinessId, cancellationToken);

        if (business is null)
        {
            return Result.Fail(
                new NotFoundError($"could not find business with id {request.BusinessId}"));
        }

        List<string> invalidImageNames = await ValidateImagesInStorage(request, cancellationToken);
        if (invalidImageNames.Count > 0)
        {
            return Result.Fail(
                new NotFoundError($"some images doesnt exist in storage. Images: {string.Join(',', invalidImageNames)}"));
        }

        business.Logo = request.Logo is not null ? request.Logo : business.Logo;
        business.Cover = request.Cover is not null ? request.Cover : business.Cover;
        business.Portfolio = request.Portfolio is not null ? request.Portfolio : business.Portfolio;
        business.UpdatedDateTime = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }

    private async Task<List<string>> ValidateImagesInStorage(UpdateBusinessImagesCommand request, CancellationToken cancellationToken)
    {
        List<string> invalidImageNames = [];

        if (request.Logo is not null)
        {
            if (!await _storageManager.IsFileExists(request.Logo.Name, cancellationToken))
            {
                invalidImageNames.Add(request.Logo.Name);
            }
        }

        if (request.Cover is not null)
        {
            if (!await _storageManager.IsFileExists(request.Cover.Name, cancellationToken))
            {
                invalidImageNames.Add(request.Cover.Name);
            }
        }

        if (request.Portfolio is not null)
        {
            foreach (var portfolio in request.Portfolio)
            {
                if (!await _storageManager.IsFileExists(portfolio.Name, cancellationToken))
                {
                    invalidImageNames.Add(portfolio.Name);
                }
            }
        }

        return invalidImageNames;
    }
}
