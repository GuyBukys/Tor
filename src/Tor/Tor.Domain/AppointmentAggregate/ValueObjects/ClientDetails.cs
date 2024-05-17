using Tor.Domain.Common.ValueObjects;

namespace Tor.Domain.AppointmentAggregate.ValueObjects;

public record ClientDetails
{
    public string Name { get; set; } = string.Empty;
    public PhoneNumber? PhoneNumber { get; set; } = null!;

    public ClientDetails(string name, PhoneNumber? phoneNumber)
    {
        Name = name;
        PhoneNumber = phoneNumber;
    }

    public ClientDetails() { }
}
