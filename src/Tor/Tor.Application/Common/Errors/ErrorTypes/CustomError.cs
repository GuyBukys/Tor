using FluentResults;

namespace Tor.Application.Common.Errors.ErrorTypes;

public class CustomError(string message) : Error(message)
{
}
