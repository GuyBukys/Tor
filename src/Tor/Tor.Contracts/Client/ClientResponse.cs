using Tor.Domain.Common.ValueObjects;

namespace Tor.Contracts.Client;

public class ClientResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateOnly? BirthDate { get; set; }
    public PhoneNumber PhoneNumber { get; set; } = default!;
    public Address? Address { get; set; } = null!;
    public Image? ProfileImage { get; set; } = null!;
}
