using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Tor.Infrastructure.Utils;

/// <summary>
/// Converts datetime from the database to a UTC datetime
/// </summary>
internal class DateTimeUtcConverter : ValueConverter<DateTime, DateTime>
{
    public DateTimeUtcConverter() : base(
            dateTimeFromCode => dateTimeFromCode,
            dateTimeFromDb => dateTimeFromDb.ToUniversalTime())
    {
    }
}
