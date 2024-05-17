namespace Tor.Infrastructure.Notifications.Models;

internal class FcmSendModel
{
    public string to { get; set; } = string.Empty;
    public FcmSendModelData data { get; set; } = default!;
}

internal class FcmSendModelData
{
    public string message { get; set; } = string.Empty;
}
