using System;
using System.Globalization;

namespace MyTimeClassifier.Utils;

/// <summary>
/// Time helper
/// </summary>
public static class TimeUtils
{
    private static readonly DateTime _jan1St1970 = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Get unix timestamp
    /// </summary>
    /// <returns>Unix timestamp</returns>
    public static ulong UnixTimeNow()
    {
        var timeSpan = DateTime.UtcNow - _jan1St1970;
        return (ulong)timeSpan.TotalSeconds;
    }

    /// <summary>
    /// Get unix timestamp in milliseconds
    /// </summary>
    /// <returns>Unix timestamp</returns>
    public static long UnixTimeNowMs()
    {
        var timeSpan = DateTime.UtcNow - _jan1St1970;
        return (long)timeSpan.TotalMilliseconds;
    }

    /// <summary>
    /// Convert UnixTimestamp to DateTime
    /// </summary>
    /// <param name="timeStamp"></param>
    /// <returns></returns>
    public static DateTime FromUnixTime(this long timeStamp) => _jan1St1970.AddSeconds(timeStamp).ToLocalTime();

    /// <summary>
    /// Convert DateTime to UnixTimestamp
    /// </summary>
    /// <param name="dateTime">The DateTime to convert</param>
    /// <returns></returns>
    public static ulong ToUnixTime(this DateTime dateTime)
        => (ulong)dateTime.ToUniversalTime().Subtract(_jan1St1970).TotalSeconds;

    /// <summary>
    /// Try parse international data
    /// </summary>
    /// <param name="input"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool TryParseInternational(this string input, out DateTime result) => DateTime.TryParse(input,
        CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out result);

    public static string ToVeryLargeTimeString(this ulong unixTimeSec)
    {
        const ulong secondsInMinute = 60;
        const ulong secondsInHour = 60 * secondsInMinute;
        const ulong secondsInDay = 24 * secondsInHour;
        const ulong secondsIn30DayMonth = 30 * secondsInDay;
        const ulong secondsIn365DayYear = 365 * secondsInDay;

        var years = unixTimeSec / secondsIn365DayYear;
        var months = unixTimeSec % secondsIn365DayYear / secondsIn30DayMonth;
        var days = unixTimeSec % secondsIn30DayMonth / secondsInDay;
        var hours = unixTimeSec % secondsInDay / secondsInHour;
        var minutes = unixTimeSec % secondsInHour / secondsInMinute;
        var seconds = unixTimeSec % secondsInMinute;

        return (years, months, days, hours, minutes, seconds) switch
        {
            (0, 0, 0, 0, 0, 0) => "0s",
            (0, 0, 0, 0, 0, _) => $"{seconds}s",
            (0, 0, 0, 0, _, _) => $"{minutes}m {seconds}s",
            (0, 0, 0, _, _, _) => $"{hours}h {minutes}m {seconds}s",
            (0, 0, _, _, _, _) => $"{days}d {hours}h {minutes}m {seconds}s",
            (0, _, _, _, _, _) => $"{months}m {days}d {hours}h {minutes}m {seconds}s",
            (_, _, _, _, _, _) => $"{years}y {months}m {days}d {hours}h {minutes}m {seconds}s"
        };
    }
}