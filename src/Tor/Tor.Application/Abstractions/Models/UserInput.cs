using Tor.Domain.Common.Enums;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.Abstractions.Models;

public record UserInput(
    string UserToken,
    PhoneNumber PhoneNumber,
    EntityType EntityType);
