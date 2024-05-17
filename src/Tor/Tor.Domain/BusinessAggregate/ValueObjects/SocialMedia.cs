using Tor.Domain.BusinessAggregate.Enums;

namespace Tor.Domain.BusinessAggregate.ValueObjects;

public record SocialMedia(
    SocialMediaType Type,
    string Url);
