namespace Tor.Contracts.Business;

public class CreateBusinessResponse
{
    public Guid Id { get; set; }
    public string InvitationId { get; set; } = string.Empty;
}
