using Tor.Application.Abstractions;
using Tor.Application.Common.Extensions;

namespace Tor.Infrastructure.Providers;

internal sealed class IsraelDateTimeProvider : IIsraelDateTimeProvider
{
    public DateTime Now => DateTimeOffset.UtcNow.ToIsraelTime();
}
