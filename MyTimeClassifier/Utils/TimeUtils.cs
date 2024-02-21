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
}
