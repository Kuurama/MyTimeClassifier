using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace MyTimeClassifier.UI.Components;

public class MultiplierTwoConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        var value1 = values[0] is string str1 ? float.Parse(str1) : (float?)values[0] ?? 0f;
        var value2 = values[1] is string str2 ? float.Parse(str2) : (float?)values[1] ?? 0f;

        if (targetType == typeof(double))
            return (double)value1 * value2;

        return value1 * value2;
    }
}

public class MultiplierThreeConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        var value1 = values[0] is string str1 ? float.Parse(str1) : (float?)values[0] ?? 0f;
        var value2 = values[1] is string str2 ? float.Parse(str2) : (float?)values[1] ?? 0f;
        var value3 = values[2] is string str3 ? float.Parse(str3) : (float?)values[2] ?? 0f;

        if (targetType == typeof(double))
            return (double)value1 * value2 * value3;

        return value1 * value2 * value3;
    }
}