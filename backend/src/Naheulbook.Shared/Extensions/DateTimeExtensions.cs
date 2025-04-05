namespace Naheulbook.Shared.Extensions;

public static class DateTimeExtensions
{
    public static DateTime RoundToSeconds(this DateTime dateTime)
    {
        return new DateTime(
            dateTime.Year,
            dateTime.Month,
            dateTime.Day,
            dateTime.Hour,
            dateTime.Minute,
            dateTime.Second,
            DateTimeKind.Utc
        );
    }
}