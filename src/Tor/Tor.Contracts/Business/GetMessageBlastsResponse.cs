using Tor.Domain.MessageBlastAggregate.Enums;

namespace Tor.Contracts.Business;

public class GetMessageBlastsResponse
{
    public List<MessageBlastResponse> MessageBlasts { get; set; } = [];
}

public class MessageBlastResponse
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string DisplayDescription { get; set; } = string.Empty;
    public MessageBlastType Type { get; set; }
    public string TemplateBody { get; set; } = string.Empty;
    public bool CanEditBody { get; set; }
    public string? BusinessTitle { get; set; }
    public string? BusinessBody { get; set; }
}
