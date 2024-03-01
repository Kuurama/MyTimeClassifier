using System.ComponentModel;

namespace DBConverter.Utils;

public static class EnumFlagHelper
{
    public static bool HasAnyFlag(this Enum? p_Value, Enum p_Flags) => p_Value != null && (Convert.ToUInt32(p_Value) & Convert.ToUInt32(p_Flags)) != 0;

    public static bool HasAnyFlag(this UInt32 p_Value, UInt32 p_Flags) => (p_Value & p_Flags) != 0;

    public static bool HasAnyFlag(this UInt32 p_Value, Enum p_Flags) => (p_Value & Convert.ToUInt32(p_Flags)) != 0;

    public static bool HasFlagCombination(this Enum? p_Value, Enum p_Flags) => p_Value != null && (Convert.ToUInt32(p_Value) & Convert.ToUInt32(p_Flags)) == Convert.ToUInt32(p_Flags);

    public static bool HasFlagCombination(this UInt32 p_Value, UInt32 p_Flags) => (p_Value & p_Flags) == p_Flags;

    public static bool HasFlagCombination(this UInt32 p_Value, Enum p_Flags) => (p_Value & Convert.ToUInt32(p_Flags)) == Convert.ToUInt32(p_Flags);

    public static T AddFlags<T>(this T p_Value, T p_Flags) where T : struct => p_Value.AddFlags(p_Flags, true);

    public static T RemoveFlags<T>(this T p_Value, T p_Flags) where T : struct => p_Value.AddFlags(p_Flags, false);

    public static uint RemoveFlags(this UInt32 p_Value, UInt32 p_Flags) => p_Value & ~p_Flags;

    public static IEnumerable<T> GetFlags<T>(this T p_Value) where T : struct
    {
        CheckIsEnum<T>(true);

        foreach (var l_Flag in Enum.GetValues(typeof(T)).Cast<T>())
        {
            if (p_Value.HasAnyFlag(l_Flag))
                yield return l_Flag;
        }
    }

    private static void CheckIsEnum<T>(bool p_WithFlags)
    {
        if (!typeof(T).IsEnum) throw new ArgumentException($"Type '{typeof(T).FullName}' is not an enum");
        if (p_WithFlags && !Attribute.IsDefined(typeof(T), typeof(FlagsAttribute))) throw new ArgumentException($"Type '{typeof(T).FullName}' doesn't have the 'Flags' attribute");
    }

    public static bool HasAnyFlag<T>(this T p_Value, T p_Flag) where T : struct
    {
        CheckIsEnum<T>(true);
        var l_Value = Convert.ToInt64(p_Value);
        var l_Flag  = Convert.ToInt64(p_Flag);
        return (l_Value & l_Flag) != 0;
    }

    public static T CombineFlags<T>(this IEnumerable<T> p_Flags) where T : struct
    {
        CheckIsEnum<T>(true);
        long l_Value = 0;

        foreach (var l_Flag in p_Flags)
        {
            var l_Int64Flag = Convert.ToInt64(l_Flag);
            l_Value |= l_Int64Flag;
        }

        return (T)Enum.ToObject(typeof(T), l_Value);
    }

    public static string? GetDescription<T>(this T p_Value) where T : struct
    {
        CheckIsEnum<T>(false);
        var l_Name = Enum.GetName(typeof(T), p_Value);

        if (l_Name != null)
        {
            var l_Field = typeof(T).GetField(l_Name);
            if (l_Field != null)
                if (Attribute.GetCustomAttribute(l_Field, typeof(DescriptionAttribute)) is DescriptionAttribute l_Attr)
                    return l_Attr.Description;
        }

        return null;
    }

    public static T AddFlags<T>(this T p_Value, T p_Flags, bool p_On) where T : struct
    {
        CheckIsEnum<T>(true);
        var l_Value = Convert.ToInt64(p_Value);
        var l_Flag  = Convert.ToInt64(p_Flags);
        if (p_On)
            l_Value |= l_Flag;
        else
            l_Value &= ~l_Flag;
        return (T)Enum.ToObject(typeof(T), l_Value);
    }
}
