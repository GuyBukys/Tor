using FluentResults;

namespace Tor.Application.Common.Errors.ErrorTypes;

public class NotFoundError : Error
{
    public NotFoundError(string message)
        : base($"not found error: {message}")
    {
    }
}
