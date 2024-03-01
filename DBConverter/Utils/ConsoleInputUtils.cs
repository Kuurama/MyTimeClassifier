using System.Globalization;
using System.Numerics;

namespace DBConverter.Utils;

/// <summary>
///     Example Usage:
///     ConsoleInputUtils.GetFromConsole
///     <string>
///         ("Please provide the path of the file to read (or simply close this program and drag and drop the file on
///         it):", "Incorrect input", ConsoleInputUtils.EParseVerificationScheme.FilePath |
///         ConsoleInputUtils.EParseVerificationScheme.MinValue, 2);
///         ConsoleInputUtils.GetFromConsole
///         <int, int>
///             ("Enter the number of parallel tasks to run (please use 12 by default, 1 min, 40 max):", "Incorrect
///             input", ConsoleInputUtils.EParseVerificationScheme.Integer |
///             ConsoleInputUtils.EParseVerificationScheme.Positive | ConsoleInputUtils.EParseVerificationScheme.MaxValue,
///             default(int), 40);
/// </summary>
public static class ConsoleInputUtils
{
    [Flags]
    public enum EParseVerificationScheme
    {
        String        = 0,
        FilePath      = 1 << 0,
        Integer       = 1 << 1,
        Long          = 1 << 2,
        Double        = 1 << 3,
        Float         = 1 << 4,
        NumberTypes   = Integer | Long | Double | Float,
        Negative      = 1 << 5,
        Positive      = 1 << 6,
        MinValue      = 1 << 7,
        MaxValue      = 1 << 8,
        DirectoryPath = 1 << 9,
        Constraints   = Negative | Positive | MinValue | MaxValue
    }

    public static void PressAnyKeyToExit()
    {
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    public static void PressAnyKeyToContinue()
    {
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    public static bool BoolFromConsole(string p_AskMessage)
    {
        Console.WriteLine(p_AskMessage);
        Console.WriteLine("Please enter 'y' for yes or 'n' for no: ");
        var l_InputString = Console.ReadLine();

        while (string.IsNullOrEmpty(l_InputString) || l_InputString != "y" && l_InputString != "n")
        {
            Console.WriteLine("The provided value is null or invalid, please type something correct");
            l_InputString = Console.ReadLine();
        }

        Console.WriteLine('\n');
        return l_InputString == "y";
    }

    public static string AskFromConsole(string p_AskMessage)
    {
        Console.WriteLine(p_AskMessage);

        var l_InputString = Console.ReadLine();

        while (string.IsNullOrEmpty(l_InputString))
        {
            Console.WriteLine("The provided string is null, please type something correct");
            l_InputString = Console.ReadLine();
        }

        return l_InputString;
    }

    public static T GetFromConsole<T>(string? p_AskMessage, string? p_FallbackMessage, EParseVerificationScheme p_ParseVerificationType, int p_MinValue = default(int), int p_MaxValue = default(int))
        => GetFromConsole<T, int>(p_AskMessage, p_FallbackMessage, p_ParseVerificationType, p_MinValue, p_MaxValue);

    public static T GetFromConsole<T, U>(string? p_AskMessage, string? p_FallbackMessage, EParseVerificationScheme p_ParseVerificationType, U? p_MinValue = default(U), U? p_MaxValue = default(U)) where U : INumber<U>
    {
        Console.WriteLine(p_AskMessage);
    GoInputString:
        var l_InputString = Console.ReadLine();

        while (l_InputString == null)
        {
            Console.WriteLine(p_FallbackMessage);
            l_InputString = Console.ReadLine();
        }

        if (VerifyParse(l_InputString, p_ParseVerificationType, out var l_ParsedObject, out var l_SucceededConstraint, p_MinValue, p_MaxValue) == false) goto GoParseFailed;

        if (l_SucceededConstraint == false)
        {
            Console.WriteLine("The constraint was not respected, please try again with other values.");
            goto GoInputString;
        }

        if (l_ParsedObject is T l_ParsedObjectAsT) return l_ParsedObjectAsT;

    GoParseFailed:
        Console.WriteLine("Parse failed, make sure you entered the requested data type correctly.");
        goto GoInputString;
    }

    public static T GetFromConsole<T>() where T : struct, Enum
    {
        Console.WriteLine("Type one of the following values:");
        var l_Enums = Enum.GetValues<T>();
        foreach (var l_T in l_Enums) Console.WriteLine($"{l_T.GetHashCode()} - {l_T}");

    GoInputString:
        var l_InputString = Console.ReadLine();

        while (l_InputString == null)
        {
            Console.WriteLine("Incorrect input, please try again.");
            l_InputString = Console.ReadLine();
        }

        if (Enum.TryParse(l_InputString, out T l_Value) == false || Enum.IsDefined(l_Value) == false) goto GoParseFailed;

        return l_Value;

    GoParseFailed:
        Console.WriteLine("Parse failed, make sure you entered the requested data type correctly.");
        goto GoInputString;
    }

    /// <summary>
    ///     Returns false when the parse fails, true when it succeeds. (Except FilePath => Returns false when the file doesn't
    ///     exist)
    /// </summary>
    /// <param name="p_Input"></param>
    /// <param name="p_ParseVerification"></param>
    /// <param name="p_ParsedInput"></param>
    /// <param name="p_SucceededConstraint"></param>
    /// <param name="p_MinValue"></param>
    /// <param name="p_MaxValue"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static bool VerifyParse<T>(string? p_Input, EParseVerificationScheme p_ParseVerification, out object? p_ParsedInput, out bool p_SucceededConstraint, T? p_MinValue = default(T), T? p_MaxValue = default(T)) where T : INumber<T>
    {
        p_ParsedInput         = null;
        p_SucceededConstraint = true;
        if (p_Input is null) return false;

        if (p_ParseVerification.HasAnyFlag(EParseVerificationScheme.String | EParseVerificationScheme.FilePath | EParseVerificationScheme.DirectoryPath))
        {
            p_ParsedInput = p_Input;

            if (p_ParseVerification.HasAnyFlag(EParseVerificationScheme.Constraints))
            {
                int.TryParse(p_MinValue?.ToString(), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var l_MinParsed);
                int.TryParse(p_MaxValue?.ToString(), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var l_MaxParsed);
                p_SucceededConstraint = CheckStringConstrains(p_Input, p_ParseVerification, l_MinParsed, l_MaxParsed);
            }

            if (p_ParseVerification.HasAnyFlag(EParseVerificationScheme.DirectoryPath)) return Directory.Exists(p_Input);

            return !p_ParseVerification.HasAnyFlag(EParseVerificationScheme.FilePath) || File.Exists(p_Input);
        }

        /// Returns true if it's not a number, and indeed not a string or file path.
        if (p_ParseVerification.HasAnyFlag(EParseVerificationScheme.NumberTypes) == false) return true;

        if (!T.TryParse(p_Input, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out var l_ParsedValue)) return false;

        p_ParsedInput = l_ParsedValue;

        if (p_ParseVerification.HasAnyFlag(EParseVerificationScheme.Constraints)) p_SucceededConstraint = CheckNumberConstrains(l_ParsedValue, p_ParseVerification, p_MinValue, p_MaxValue);
        return true;
    }

    private static bool CheckStringConstrains<T>(string p_Input, EParseVerificationScheme p_ParseVerificationScheme, T? p_MinValue = default(T), T? p_MaxValue = default(T)) where T : INumber<T>
    {
        if (p_MinValue is null && p_MaxValue is null) return !p_ParseVerificationScheme.HasAnyFlag(EParseVerificationScheme.MinValue | EParseVerificationScheme.MaxValue);

        if (p_ParseVerificationScheme.HasAnyFlag(EParseVerificationScheme.MinValue))
        {
            if (p_MinValue is null) return false;

            if (!T.TryParse(p_Input.Length.ToString(), NumberStyles.Number, NumberFormatInfo.InvariantInfo, out var l_ParsedValue)) return false;

            if (l_ParsedValue < p_MinValue) return false;
        }

        if (p_ParseVerificationScheme.HasAnyFlag(EParseVerificationScheme.MaxValue))
        {
            if (p_MaxValue is null) return false;

            if (!T.TryParse(p_Input.Length.ToString(), NumberStyles.Number, NumberFormatInfo.InvariantInfo, out var l_ParsedValue)) return false;

            if (l_ParsedValue > p_MaxValue) return false;
        }

        return true;
    }

    private static bool CheckNumberConstrains<T>(T p_Number, EParseVerificationScheme p_ParseVerificationScheme, T? p_NumberMin = default(T), T? p_NumberMax = default(T)) where T : INumber<T>
    {
        if (p_ParseVerificationScheme.HasAnyFlag(EParseVerificationScheme.Negative) && p_Number >= T.Zero) return false;
        if (p_ParseVerificationScheme.HasAnyFlag(EParseVerificationScheme.Positive) && p_Number <= T.Zero) return false;

        var l_CheckMaxValue = p_ParseVerificationScheme.HasAnyFlag(EParseVerificationScheme.MaxValue);
        if (p_NumberMax is null && l_CheckMaxValue || l_CheckMaxValue && p_Number > p_NumberMax!) return false;

        var l_CheckMinValue = p_ParseVerificationScheme.HasAnyFlag(EParseVerificationScheme.MinValue);
        return (p_NumberMin is not null || !l_CheckMinValue) && (!l_CheckMinValue || p_Number >= p_NumberMin!);
    }
}
