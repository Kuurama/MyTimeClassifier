using System;
using System.Globalization;

namespace MyTimeClassifier.Utils;

/// <summary>
///     Time helper
/// </summary>
public static class TimeUtils
{
    private static readonly DateTime s_Jan1St1970 = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///     Get unix timestamp
    /// </summary>
    /// <returns>Unix timestamp</returns>
    public static UInt64 UnixTimeNow()
    {
        var l_TimeSpan = DateTime.UtcNow - s_Jan1St1970;
        return (UInt64)l_TimeSpan.TotalSeconds;
    }

    /// <summary>
    ///     Get unix timestamp in milliseconds
    /// </summary>
    /// <returns>Unix timestamp</returns>
    public static Int64 UnixTimeNowMS()
    {
        var l_TimeSpan = DateTime.UtcNow - s_Jan1St1970;
        return (Int64)l_TimeSpan.TotalMilliseconds;
    }

    /// <summary>
    ///     Convert UnixTimestamp to DateTime
    /// </summary>
    /// <param name="p_TimeStamp"></param>
    /// <returns></returns>
    public static DateTime FromUnixTime(this Int64 p_TimeStamp) => s_Jan1St1970.AddSeconds(p_TimeStamp).ToLocalTime();

    /// <summary>
    ///     Convert DateTime to UnixTimestamp
    /// </summary>
    /// <param name="p_DateTime">The DateTime to convert</param>
    /// <returns></returns>
    public static UInt64 ToUnixTime(this DateTime p_DateTime) => (UInt64)p_DateTime.ToUniversalTime().Subtract(s_Jan1St1970).TotalSeconds;

    /// <summary>
    ///     Try parse international data
    /// </summary>
    /// <param name="p_Input"></param>
    /// <param name="p_Result"></param>
    /// <returns></returns>
    public static bool TryParseInternational(this string p_Input, out DateTime p_Result) => DateTime.TryParse(p_Input, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out p_Result);

    public static string ToVeryLargeTimeString(this UInt64 p_UnixTimeSec)
    {
        const UInt64 SECONDS_IN_MINUTE       = 60;
        const UInt64 SECONDS_IN_HOUR         = 60  * SECONDS_IN_MINUTE;
        const UInt64 SECONDS_IN_DAY          = 24  * SECONDS_IN_HOUR;
        const UInt64 SECONDS_IN_30_DAY_MONTH = 30  * SECONDS_IN_DAY;
        const UInt64 SECONDS_IN_365_DAY_YEAR = 365 * SECONDS_IN_DAY;

        var l_Years   = p_UnixTimeSec                           / SECONDS_IN_365_DAY_YEAR;
        var l_Months  = p_UnixTimeSec % SECONDS_IN_365_DAY_YEAR / SECONDS_IN_30_DAY_MONTH;
        var l_Days    = p_UnixTimeSec % SECONDS_IN_30_DAY_MONTH / SECONDS_IN_DAY;
        var l_Hours   = p_UnixTimeSec % SECONDS_IN_DAY          / SECONDS_IN_HOUR;
        var l_Minutes = p_UnixTimeSec % SECONDS_IN_HOUR         / SECONDS_IN_MINUTE;
        var l_Seconds = p_UnixTimeSec                           % SECONDS_IN_MINUTE;

        var l_Tuple = (l_Years, l_Months, l_Days, l_Hours, l_Minutes, l_Seconds);

        return l_Tuple switch
        {
            (0, 0, 0, 0, 0, 0) => "0s",
            (0, 0, 0, 0, 0, _) => $"{l_Seconds}s",
            (0, 0, 0, 0, _, _) => $"{l_Minutes}m {l_Seconds}s",
            (0, 0, 0, _, _, _) => $"{l_Hours}h {l_Minutes}m {l_Seconds}s",
            (0, 0, _, _, _, _) => $"{l_Days}d {l_Hours}h {l_Minutes}m {l_Seconds}s",
            (0, _, _, _, _, _) => $"{l_Months}m {l_Days}d {l_Hours}h {l_Minutes}m {l_Seconds}s",
            (_, _, _, _, _, _) => $"{l_Years}y {l_Months}m {l_Days}d {l_Hours}h {l_Minutes}m {l_Seconds}s"
        };
    }
}
