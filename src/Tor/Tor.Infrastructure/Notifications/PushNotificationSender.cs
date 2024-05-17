using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;

namespace Tor.Infrastructure.Notifications;

internal sealed class PushNotificationSender : IPushNotificationSender
{
    private readonly FcmSettings _fcmSettings;
    private readonly ILogger<PushNotificationSender> _logger;

    public PushNotificationSender(
        IOptions<FcmSettings> fcmSettings,
        ILogger<PushNotificationSender> logger)
    {
        _fcmSettings = fcmSettings.Value;
        _logger = logger;
    }

    public async Task<SendPushNotificationResponse> Send(
        SendPushNotificationRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.DeviceTokens.Any())
        {
            _logger.LogWarning("request has no device tokens. request: {@request}", request);
            return new SendPushNotificationResponse();
        }

        _logger.LogInformation("device tokens: {@deviceTokens}", request.DeviceTokens);

        MulticastMessage multicastMessage = new()
        {
            Tokens = request.DeviceTokens.ToList(),
            Notification = new Notification
            {
                Title = request.Title,
                Body = request.Message,
            },
            Data = request.Metadata,
        };

        BatchResponse response = await FirebaseMessaging.DefaultInstance
            .SendMulticastAsync(multicastMessage, dryRun: _fcmSettings.IsTestMode, cancellationToken);

        if (response.SuccessCount != multicastMessage.Tokens.Count)
        {
            _logger.LogError("some notifications failed to be sent. failure: {faliureCount}, success: {successCount}, responses: {@responses}",
                response.FailureCount,
                response.SuccessCount,
                response.Responses.Select(x => x.Exception is not null ?
                    $"{x.Exception.MessagingErrorCode}: {x.Exception.MessagingErrorCode}" :
                    "no exception"));
        }

        IEnumerable<string> messageIds = response.Responses
            .Where(x => x.IsSuccess)
            .Select(x => x.MessageId);

        return new SendPushNotificationResponse
        {
            MessageIds = messageIds
        };
    }

    public async Task<AddToGroupResponse> AddToGroup(AddToGroupRequest request)
    {
        List<string> deviceTokens = request.DeviceTokens.ToList();

        _logger.LogInformation("device tokens: {@deviceTokens}", deviceTokens);

        var response = await FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync(
            deviceTokens,
            request.GroupName);

        if (response.SuccessCount != deviceTokens.Count)
        {
            _logger.LogError("some tokens failed to be subscribed to topic. response: {@subscribeResponse}", response);
        }

        return new AddToGroupResponse
        {
            SuccessCount = response.SuccessCount,
        };
    }

    public async Task<RemoveFromGroupResponse> RemoveFromGroup(RemoveFromGroupRequest request)
    {
        List<string> deviceTokens = request.DeviceTokens.ToList();

        _logger.LogInformation("device tokens: {@deviceTokens}", deviceTokens);

        var response = await FirebaseMessaging.DefaultInstance.UnsubscribeFromTopicAsync(
            deviceTokens,
            request.GroupName);

        if (response.SuccessCount != deviceTokens.Count)
        {
            _logger.LogError("some tokens failed to be unsubscribed to topic. response: {@subscribeResponse}", response);
        }

        return new RemoveFromGroupResponse
        {
            SuccessCount = response.SuccessCount,
        };
    }

    public async Task<SendToGroupResponse> SendToGroup(SendToGroupRequest request, CancellationToken cancellationToken)
    {
        Message message = new()
        {
            Topic = request.GroupName,
            Notification = new()
            {
                Title = request.Title,
                Body = request.Message,
            }
        };

        var response = await FirebaseMessaging.DefaultInstance.SendAsync(message, cancellationToken);

        return new SendToGroupResponse
        {
            MessageId = response,
        };
    }
}
