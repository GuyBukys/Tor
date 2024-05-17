namespace Tor.Infrastructure.Common.Exceptions;

internal class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base($"not found exception: {message}") { }
}
