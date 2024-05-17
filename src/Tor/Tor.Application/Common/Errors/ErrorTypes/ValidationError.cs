using FluentResults;

namespace Tor.Application.Common.Errors.ErrorTypes;

public class ValidationError : Error
{
    public string PropertyName { get; init; }

    public ValidationError(string propertyName, string message)
        : base($"validation error: {message}")
    {
        PropertyName = propertyName;
    }
}
