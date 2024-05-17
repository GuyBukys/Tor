using FluentResults;

namespace Tor.Application.Common.Errors.ErrorTypes;

public class DomainError : Error
{
    public DomainError(string message)
        : base($"domain error: {message}")
    {
    }
}
