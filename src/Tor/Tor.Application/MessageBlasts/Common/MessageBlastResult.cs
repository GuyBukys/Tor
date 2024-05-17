using Tor.Domain.MessageBlastAggregate.Entities;
using Tor.Domain.MessageBlastAggregate.Enums;

namespace Tor.Application.MessageBlasts.Common;

public record MessageBlastResult(
    Guid Id,
    bool IsActive,
    string Name,
    string Description,
    string DisplayName,
    string DisplayDescription,
    MessageBlastType Type,
    string TemplateBody,
    bool CanEditBody,
    string? BusinessBody)
{
    public static MessageBlastResult FromBusinessMessageBlast(BusinessMessageBlast businessMessageBlast)
    {
        return new MessageBlastResult(
            businessMessageBlast.MessageBlast.Id,
            businessMessageBlast.IsActive,
            businessMessageBlast.MessageBlast.Name,
            businessMessageBlast.MessageBlast.Description,
            businessMessageBlast.MessageBlast.DisplayName,
            businessMessageBlast.MessageBlast.DisplayDescription,
            businessMessageBlast.MessageBlast.Type,
            businessMessageBlast.MessageBlast.TemplateBody,
            businessMessageBlast.MessageBlast.CanEditBody,
            businessMessageBlast.Body);
    }
};

