using System;

public static class DateTimeExtensions
{
    private const string IsoDateFormat = "yyyy-MM-dd";
    private static readonly TimeZoneInfo SwedishTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");

    public static string ToSwedishLocalString(this DateTime dateTime)
    {
        return dateTime.FromUtcToSwedishTimeZone().ToString($"{IsoDateFormat} HH:mm");
    }

    public static string ToSwedishLocalDateOnlyString(this DateTime dateTime)
    {
        return dateTime.ToString(IsoDateFormat);
    }

    public static DateTime FromUtcToSwedishTimeZone(this DateTime dateTime)
    {
        return TimeZoneInfo.ConvertTime(dateTime, sourceTimeZone: TimeZoneInfo.Utc, destinationTimeZone: SwedishTimeZoneInfo);
    }

    public static DateTime FromSwedishTimeZoneToUtc(this DateTime dateTime)
    {
        return TimeZoneInfo.ConvertTime(dateTime, sourceTimeZone: SwedishTimeZoneInfo, destinationTimeZone: TimeZoneInfo.Utc);
    }

    public static DateTime ToUtc(this DateTime dateTime)
    {
        return new DateTime(
            year: dateTime.Year, month: dateTime.Month, day: dateTime.Day,
            hour: dateTime.Hour, minute: dateTime.Minute, second: dateTime.Second,
            millisecond: dateTime.Millisecond,
            kind: DateTimeKind.Utc);
    }
}
