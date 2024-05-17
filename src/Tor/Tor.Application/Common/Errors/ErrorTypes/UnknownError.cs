using FluentResults;

namespace Tor.Application.Common.Errors.ErrorTypes;

public class UnknownError : Error
{
    public UnknownError(string message)
        : base($"unknown error: {message}")
    {
    }
}
