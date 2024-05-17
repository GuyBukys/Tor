using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Domain.AppointmentAggregate.ValueObjects;

public class ServiceDetails
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public AmountDetails Amount { get; set; } = default!;
    public List<Duration> Durations { get; set; } = [];

    public ServiceDetails(
        string name,
        string description,
        AmountDetails amount,
        List<Duration> durations)
    {
        Name = name;
        Description = description;
        Amount = amount;
        Durations = durations;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not ServiceDetails otherServiceDetails)
        {
            return false;
        }

        return
            Name == otherServiceDetails.Name &&
            Description == otherServiceDetails.Description &&
            Amount == otherServiceDetails.Amount &&
            Durations.SequenceEqual(otherServiceDetails.Durations);
    }

    public override int GetHashCode()
    {
        int amountHashCode = Amount?.GetHashCode() ?? 0;
        return Name.GetHashCode() + Description.GetHashCode() + amountHashCode + Durations.Sum(x => x.GetHashCode());
    }

    public override string ToString()
    {
        return $"Name: {Name}. Description: {Description}. Amount: {Amount}. Durations: {Durations}";
    }

    public ServiceDetails() { }
};
