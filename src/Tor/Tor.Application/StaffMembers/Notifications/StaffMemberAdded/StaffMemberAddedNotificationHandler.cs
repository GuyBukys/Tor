using MediatR;
using Microsoft.Extensions.Logging;
using Tor.Application.Abstractions;

namespace Tor.Application.StaffMembers.Notifications.StaffMemberAdded;

public sealed class StaffMemberAddedNotificationHandler : INotificationHandler<StaffMemberAddedNotification>
{
    private readonly ITorDbContext _context;
    private readonly IPushNotificationSender _pushNotificationSender;
    private readonly ILogger<StaffMemberAddedNotificationHandler> _logger;

    public StaffMemberAddedNotificationHandler(
        ITorDbContext context,
        IPushNotificationSender pushNotificationSender,
        ILogger<StaffMemberAddedNotificationHandler> logger)
    {
        _context = context;
        _pushNotificationSender = pushNotificationSender;
        _logger = logger;
    }

    public Task Handle(StaffMemberAddedNotification notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
