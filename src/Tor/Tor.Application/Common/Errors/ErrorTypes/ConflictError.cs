using FluentResults;

namespace Tor.Application.Common.Errors.ErrorTypes;

public class ConflictError : Error
{
    public ConflictError(string message)
        : base($"conflict error: {message}")
    {
    }
}
