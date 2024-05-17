using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.UserAggregate;

namespace Tor.Application.MessageBlasts.Commands.BulkSendNotification;

internal sealed class BulkSendNotificationCommandHandler : IRequestHandler<BulkSendNotificationCommand, Result>
{
    private readonly ITorDbContext _context;
    private readonly IPushNotificationSender _pushNotificationSender;

    public BulkSendNotificationCommandHandler(
        ITorDbContext context,
        IPushNotificationSender pushNotificationSender)
    {
        _context = context;
        _pushNotificationSender = pushNotificationSender;
    }

    public async Task<Result> Handle(BulkSendNotificationCommand request, CancellationToken cancellationToken)
    {
        bool isBusinessExists = await _context.Businesses
            .AnyAsync(x => x.Id == request.BusinessId, cancellationToken);
        if (!isBusinessExists)
        {
            return Result.Fail(
                new NotFoundError($"could not find business with id {request.BusinessId}"));
        }

        List<User> users = await _context.Businesses
            .Where(x => x.Id == request.BusinessId)
            .SelectMany(x => x.Clients)
            .Select(x => x.User)
            .ToListAsync(cancellationToken);

        List<User> staffMemberUsers = await _context.StaffMembers
            .Where(x => x.BusinessId == request.BusinessId)
            .Select(x => x.User)
            .ToListAsync(cancellationToken);

        IEnumerable<string> deviceTokens = users
            .Union(staffMemberUsers)
            .SelectMany(x => x.Devices)
            .Select(x => x.Token);

        SendPushNotificationRequest pushNotificationRequest = new(
            deviceTokens,
            request.Title,
            request.Message);
        await _pushNotificationSender.Send(pushNotificationRequest, cancellationToken);

        return Result.Ok();
    }
}
