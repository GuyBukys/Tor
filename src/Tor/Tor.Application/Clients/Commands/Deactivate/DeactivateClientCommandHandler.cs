using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Clients.Notifications.ClientDeactivated;
using Tor.Application.Common.Errors.ErrorTypes;

namespace Tor.Application.Clients.Commands.Deactivate;

internal sealed class DeactivateClientCommandHandler : IRequestHandler<DeactivateClientCommand, Result>
{
    private readonly ITorDbContext _context;
    private readonly IClientRepository _repository;
    private readonly IPublisher _publisher;

    public DeactivateClientCommandHandler(ITorDbContext context, IClientRepository repository, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
        _repository = repository;
    }

    public async Task<Result> Handle(DeactivateClientCommand request, CancellationToken cancellationToken)
    {
        bool isClientExists = await _context.Clients
            .AnyAsync(x => x.Id == request.ClientId, cancellationToken);
        if (!isClientExists)
        {
            return Result.Fail(
                new NotFoundError($"could not find client with id {request.ClientId}"));
        }

        List<string>? deviceTokens = (await _context.Users
            .AsNoTracking()
            .Where(x => x.EntityId == request.ClientId)
            .Select(x => x.Devices)
            .FirstOrDefaultAsync(cancellationToken))!
            .ConvertAll(x => x.Token);

        await _repository.Deactivate(request.ClientId, cancellationToken);

        await _publisher.Publish(new ClientDeactivatedNotification(deviceTokens), cancellationToken);

        return Result.Ok();
    }
}
