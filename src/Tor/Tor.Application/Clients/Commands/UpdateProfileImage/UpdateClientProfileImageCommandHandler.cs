using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.Common.Enums;

namespace Tor.Application.Clients.Commands.UpdateProfileImage;

internal sealed class UpdateClientProfileImageCommandHandler : IRequestHandler<UpdateClientProfileImageCommand, Result>
{
    private readonly ITorDbContext _context;
    private readonly IStorageManager _storageManager;

    public UpdateClientProfileImageCommandHandler(ITorDbContext context, IStorageManager storageManager)
    {
        _context = context;
        _storageManager = storageManager;
    }

    public async Task<Result> Handle(UpdateClientProfileImageCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients
            .FirstOrDefaultAsync(x => x.Id == request.ClientId, cancellationToken);

        if (client is null)
        {
            return Result.Fail(
                new NotFoundError($"could not find client with id {request.ClientId}"));
        }

        if (request.ProfileImage is not null &&
            !await _storageManager.IsFileExists(request.ProfileImage.Name, cancellationToken))
        {
            return Result.Fail(
                new NotFoundError($"profile image {request.ProfileImage.Name} doesnt exist in storage"));
        }

        client.ProfileImage = request.ProfileImage ?? _storageManager.GetDefaultImage(ImageType.Profile, EntityType.Client);
        client.UpdatedDateTime = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
