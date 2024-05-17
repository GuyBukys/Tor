namespace Tor.Application.Common.Extensions;

public static class DateTimeOffsetExtensions
{
    public static DateTime ToIsraelTime(this DateTimeOffset dateTimeOffset)
    {
        TimeZoneInfo israelTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
        return TimeZoneInfo.ConvertTimeFromUtc(dateTimeOffset.UtcDateTime, israelTimeZone);
    }

    public static DateTime ToIsraelTime(this DateTime dateTime)
    {
        TimeZoneInfo israelTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
        return TimeZoneInfo.ConvertTimeFromUtc(dateTime, israelTimeZone);
    }
}
