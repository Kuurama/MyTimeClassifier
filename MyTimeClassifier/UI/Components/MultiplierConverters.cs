using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace MyTimeClassifier.UI.Components;

public class MultiplierTwoConverter : IMultiValueConverter
{
    public object Convert(IList<object?> p_Values, Type p_TargetType, object? p_Parameter, CultureInfo p_Culture)
    {
        var l_Value1 = p_Values[0] is string l_Str1 ? float.Parse(l_Str1) : (float?)p_Values[0] ?? 0f;
        var l_Value2 = p_Values[1] is string l_Str2 ? float.Parse(l_Str2) : (float?)p_Values[1] ?? 0f;

        return l_Value1 * l_Value2;
    }
}

public class MultiplierThreeConverter : IMultiValueConverter
{
    public object Convert(IList<object?> p_Values, Type p_TargetType, object? p_Parameter, CultureInfo p_Culture)
    {
        var l_Value1 = p_Values[0] is string l_Str1 ? float.Parse(l_Str1) : (float?)p_Values[0] ?? 0f;
        var l_Value2 = p_Values[1] is string l_Str2 ? float.Parse(l_Str2) : (float?)p_Values[1] ?? 0f;
        var l_Value3 = p_Values[2] is string l_Str3 ? float.Parse(l_Str3) : (float?)p_Values[2] ?? 0f;

        return l_Value1 * l_Value2 * l_Value3;
    }
}
