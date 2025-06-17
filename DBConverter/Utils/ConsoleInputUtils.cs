using System.Globalization;
using System.Numerics;

namespace DBConverter.Utils;

/// <summary>
/// Example Usage:
/// ConsoleInputUtils.GetFromConsole{string}
/// ("Please provide the path of the file to read (or simply close this program and drag and drop the file on it):",
/// "Incorrect input", ConsoleInputUtils.EParseVerificationScheme.FilePath |
/// ConsoleInputUtils.EParseVerificationScheme.MinValue, 2);
/// ConsoleInputUtils.GetFromConsole{int, int}
/// ("Enter the number of parallel tasks to run (please use 12 by default, 1 min, 40 max):", "Incorrect
/// input", ConsoleInputUtils.EParseVerificationScheme.Integer |
/// ConsoleInputUtils.EParseVerificationScheme.Positive | ConsoleInputUtils.EParseVerificationScheme.MaxValue,
/// default(int), 40);
/// </summary>
public static class ConsoleInputUtils
{
    [Flags]
    public enum EParseVerificationScheme
    {
        String = 0,
        FilePath = 1 << 0,
        Integer = 1 << 1,
        Long = 1 << 2,
        Double = 1 << 3,
        Float = 1 << 4,
        NumberTypes = Integer | Long | Double | Float,
        Negative = 1 << 5,
        Positive = 1 << 6,
        MinValue = 1 << 7,
        MaxValue = 1 << 8,
        DirectoryPath = 1 << 9,
        Constraints = Negative | Positive | MinValue | MaxValue
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

    public static bool BoolFromConsole(string askMessage)
    {
        Console.WriteLine(askMessage);
        Console.WriteLine("Please enter 'y' for yes or 'n' for no: ");
        var inputString = Console.ReadLine();

        while (string.IsNullOrEmpty(inputString) || inputString != "y" && inputString != "n")
        {
            Console.WriteLine("The provided value is null or invalid, please type something correct");
            inputString = Console.ReadLine();
        }

        Console.WriteLine('\n');
        return inputString == "y";
    }

    public static string AskFromConsole(string askMessage)
    {
        Console.WriteLine(askMessage);

        var inputString = Console.ReadLine();

        while (string.IsNullOrEmpty(inputString))
        {
            Console.WriteLine("The provided string is null, please type something correct");
            inputString = Console.ReadLine();
        }

        return inputString;
    }

    public static T GetFromConsole<T>(
        string? askMessage, string? fallbackMessage,
        EParseVerificationScheme parseVerificationType, int minValue = default,
        int maxValue = 0)
        => GetFromConsole<T, int>(askMessage, fallbackMessage, parseVerificationType, minValue, maxValue);

    public static T GetFromConsole<T, U>(
        string? askMessage, string? fallbackMessage,
        EParseVerificationScheme parseVerificationType, U? minValue = default,
        U? maxValue = default) where U : INumber<U>
    {
        Console.WriteLine(askMessage);
    GoInputString:
        var inputString = Console.ReadLine();

        while (inputString == null)
        {
            Console.WriteLine(fallbackMessage);
            inputString = Console.ReadLine();
        }

        if (VerifyParse(inputString, parseVerificationType, out var parsedObject, out var succeededConstraint,
                minValue, maxValue) == false) goto GoParseFailed;

        if (succeededConstraint == false)
        {
            Console.WriteLine("The constraint was not respected, please try again with other values.");
            goto GoInputString;
        }

        if (parsedObject is T parsedObjectAsT) return parsedObjectAsT;

    GoParseFailed:
        Console.WriteLine("Parse failed, make sure you entered the requested data type correctly.");
        goto GoInputString;
    }

    public static T GetFromConsole<T>() where T : struct, Enum
    {
        Console.WriteLine("Type one of the following values:");
        var enums = Enum.GetValues<T>();
        foreach (var t in enums) Console.WriteLine($"{t.GetHashCode()} - {t}");

    GoInputString:
        var inputString = Console.ReadLine();

        while (inputString == null)
        {
            Console.WriteLine("Incorrect input, please try again.");
            inputString = Console.ReadLine();
        }

        if (Enum.TryParse(inputString, out T value) == false || Enum.IsDefined(value) == false)
            goto GoParseFailed;

        return value;

    GoParseFailed:
        Console.WriteLine("Parse failed, make sure you entered the requested data type correctly.");
        goto GoInputString;
    }

    /// <summary>
    /// Returns false when the parse fails, true when it succeeds. (Except FilePath => Returns false when the file doesn't
    /// exist)
    /// </summary>
    /// <param name="input"></param>
    /// <param name="parseVerification"></param>
    /// <param name="parsedInput"></param>
    /// <param name="succeededConstraint"></param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static bool VerifyParse<T>(string? input, EParseVerificationScheme parseVerification,
                                       out object? parsedInput, out bool succeededConstraint,
                                       T? minValue = default, T? maxValue = default) where T : INumber<T>
    {
        parsedInput = null;
        succeededConstraint = true;
        if (input is null) return false;

        if (parseVerification.HasAnyFlag(EParseVerificationScheme.String | EParseVerificationScheme.FilePath |
                                         EParseVerificationScheme.DirectoryPath))
        {
            parsedInput = input;

            if (parseVerification.HasAnyFlag(EParseVerificationScheme.Constraints))
            {
                int.TryParse(minValue?.ToString(), NumberStyles.Integer, NumberFormatInfo.InvariantInfo,
                    out var minParsed);
                int.TryParse(maxValue?.ToString(), NumberStyles.Integer, NumberFormatInfo.InvariantInfo,
                    out var maxParsed);
                succeededConstraint = CheckStringConstrains(input, parseVerification, minParsed, maxParsed);
            }

            if (parseVerification.HasAnyFlag(EParseVerificationScheme.DirectoryPath))
                return Directory.Exists(input);

            return !parseVerification.HasAnyFlag(EParseVerificationScheme.FilePath) || File.Exists(input);
        }

        // Returns true if it's not a number, and indeed not a string or file path.
        if (parseVerification.HasAnyFlag(EParseVerificationScheme.NumberTypes) == false) return true;

        if (!T.TryParse(input, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out var parsedValue))
            return false;

        parsedInput = parsedValue;

        if (parseVerification.HasAnyFlag(EParseVerificationScheme.Constraints))
            succeededConstraint = CheckNumberConstrains(parsedValue, parseVerification, minValue, maxValue);
        return true;
    }

    private static bool CheckStringConstrains<T>(string input, EParseVerificationScheme parseVerificationScheme,
                                                 T? minValue = default, T? maxValue = default) where T : INumber<T>
    {
        if (minValue is null && maxValue is null)
            return !parseVerificationScheme.HasAnyFlag(EParseVerificationScheme.MinValue |
                                                       EParseVerificationScheme.MaxValue);

        if (parseVerificationScheme.HasAnyFlag(EParseVerificationScheme.MinValue))
        {
            if (minValue is null) return false;

            if (!T.TryParse(input.Length.ToString(), NumberStyles.Number, NumberFormatInfo.InvariantInfo,
                    out var parsedValue)) return false;

            if (parsedValue < minValue) return false;
        }

        if (parseVerificationScheme.HasAnyFlag(EParseVerificationScheme.MaxValue))
        {
            if (maxValue is null) return false;

            if (!T.TryParse(input.Length.ToString(), NumberStyles.Number, NumberFormatInfo.InvariantInfo,
                    out var parsedValue)) return false;

            if (parsedValue > maxValue) return false;
        }

        return true;
    }

    private static bool CheckNumberConstrains<T>(
        T number, EParseVerificationScheme parseVerificationScheme,
        T? numberMin = default, T? numberMax = default)
        where T : INumber<T>
    {
        if (parseVerificationScheme.HasAnyFlag(EParseVerificationScheme.Negative) && number >= T.Zero) return false;
        if (parseVerificationScheme.HasAnyFlag(EParseVerificationScheme.Positive) && number <= T.Zero) return false;

        var checkMaxValue = parseVerificationScheme.HasAnyFlag(EParseVerificationScheme.MaxValue);
        if (numberMax is null && checkMaxValue || checkMaxValue && number > numberMax!) return false;

        var checkMinValue = parseVerificationScheme.HasAnyFlag(EParseVerificationScheme.MinValue);
        return (numberMin is not null || !checkMinValue) && (!checkMinValue || number >= numberMin!);
    }
}