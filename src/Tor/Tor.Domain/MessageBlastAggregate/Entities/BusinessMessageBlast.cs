using Tor.Domain.BusinessAggregate;

namespace Tor.Domain.MessageBlastAggregate.Entities;

public sealed class BusinessMessageBlast
{
    public Guid BusinessId { get; set; }
    public Guid MessageBlastId { get; set; }
    public bool IsActive { get; set; }
    public string? Body { get; set; }

    public Business Business { get; set; } = default!;
    public MessageBlast MessageBlast { get; set; } = default!;
}
