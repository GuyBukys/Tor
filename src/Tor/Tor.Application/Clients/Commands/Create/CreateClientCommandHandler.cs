using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Clients.Notifications.ClientCreated;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.ClientAggregate;
using Tor.Domain.Common.Enums;
using Tor.Domain.UserAggregate;
using Tor.Domain.UserAggregate.Enum;

namespace Tor.Application.Clients.Commands.Create;

internal sealed class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, Result<Client>>
{
    private readonly ITorDbContext _context;
    private readonly IStorageManager _storageManager;
    private readonly IPublisher _publisher;

    public CreateClientCommandHandler(
        ITorDbContext context,
        IStorageManager storageManager,
        IPublisher publisher)
    {
        _context = context;
        _storageManager = storageManager;
        _publisher = publisher;
    }

    public async Task<Result<Client>> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        User? user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Fail<Client>(
                new DomainError($"user id {request.UserId} doesnt exist"));
        }

        if (user.AppType != AppType.ClientApp)
        {
            return Result.Fail<Client>(
                new DomainError("user is not a ClientApp user type"));
        }

        if (user.EntityId.HasValue)
        {
            return Result.Fail<Client>(
                new DomainError($"user alrady in use with id {user.EntityId.Value}"));
        }

        bool isImageNotInStorage = request.ProfileImage is not null &&
            !await _storageManager.IsFileExists(request.ProfileImage.Name, cancellationToken);
        if (isImageNotInStorage)
        {
            return Result.Fail<Client>(
                new DomainError("profile image does not exist in storage"));
        }

        Client client = new(Guid.NewGuid())
        {
            Name = request.Name,
            Email = request.Email,
            Address = request.Address,
            PhoneNumber = request.PhoneNumber,
            BirthDate = request.BirthDate,
            IsActive = true,
            CreatedDateTime = DateTime.UtcNow,
            UpdatedDateTime = DateTime.UtcNow,
            UserId = request.UserId,
            ProfileImage = request.ProfileImage ?? _storageManager.GetDefaultImage(ImageType.Profile, EntityType.Client),
        };

        user.EntityId = client.Id;

        await _context.Clients.AddAsync(client, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var notification = new ClientCreatedNotification(client.Id, user.Devices);
        await _publisher.Publish(notification, cancellationToken);

        return client;
    }
}
