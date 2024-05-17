using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Tor.Infrastructure.Utils;

/// <summary>
/// Converts <see cref="TimeOnly" /> to <see cref="TimeSpan"/> and vice versa.
/// </summary>
internal class TimeOnlyConverter : ValueConverter<TimeOnly, TimeSpan>
{
    /// <summary>
    /// Creates a new instance of this converter.
    /// </summary>
    public TimeOnlyConverter() : base(
            to => to.ToTimeSpan(),
            ts => TimeOnly.FromTimeSpan(ts))
    { }
}
