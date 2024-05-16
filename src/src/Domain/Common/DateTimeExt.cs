namespace Domain.Common;

public static class DateTimeExt
{
    public static long ToUnixMs(this DateTime dateTime) => new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();

    public static DateTime ToUtcDateTime(this long timestamp) => DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime;
}