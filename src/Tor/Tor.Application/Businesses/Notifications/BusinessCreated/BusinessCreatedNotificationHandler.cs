using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Application.Common.Settings;
using Tor.Domain.MessageBlastAggregate.Entities;

namespace Tor.Application.Businesses.Notifications.BusinessCreated;

internal sealed class BusinessCreatedNotificationHandler : INotificationHandler<BusinessCreatedNotification>
{
    private readonly ITorDbContext _context;
    private readonly IPushNotificationSender _pushNotificationSender;
    private readonly TwoKingsSettings _settings;
    private readonly ILogger<BusinessCreatedNotificationHandler> _logger;

    public BusinessCreatedNotificationHandler(
        ITorDbContext context,
        IPushNotificationSender pushNotificationSender,
        IOptions<TwoKingsSettings> settings,
        ILogger<BusinessCreatedNotificationHandler> logger)
    {
        _context = context;
        _pushNotificationSender = pushNotificationSender;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task Handle(BusinessCreatedNotification notification, CancellationToken cancellationToken)
    {
        await AddMessageBlasts(notification, cancellationToken);

        await AddToBusinessesGroup(notification.Devices.Select(x => x.Token));

        await SendChaChingMessage(notification.BusinessName, cancellationToken);
    }

    private async Task AddToBusinessesGroup(IEnumerable<string> deviceTokens)
    {
        AddToGroupRequest addToBusinessesGroupRequest = new(
            deviceTokens,
            Constants.Groups.BusinessesNotificationGroup);

        await _pushNotificationSender.AddToGroup(addToBusinessesGroupRequest);
    }

    private async Task AddMessageBlasts(BusinessCreatedNotification notification, CancellationToken cancellationToken)
    {
        List<BusinessMessageBlast> businessMessageBlast = await _context.MessageBlasts
            .Select(x => new BusinessMessageBlast
            {
                MessageBlastId = x.Id,
                BusinessId = notification.BusinessId,
                IsActive = true,
            })
            .ToListAsync(cancellationToken);

        await _context.BusinessMessageBlasts.AddRangeAsync(businessMessageBlast, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task SendChaChingMessage(string businessName, CancellationToken cancellationToken)
    {
        string message = $"עסק חדש נרשם לתור. שם העסק: {businessName}";

        _logger.LogInformation("sending cha ching message for device tokens: {@settings}", _settings);

        SendPushNotificationRequest request = new(
            _settings.Devices,
            "Cha Ching",
            message);

        await _pushNotificationSender.Send(request, cancellationToken);
    }
}
