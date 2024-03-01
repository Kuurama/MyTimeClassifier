using Avalonia.Data.Converters;
using Avalonia.Media;
using LiveChartsCore.SkiaSharpView.Painting;
using MyTimeClassifier.Utils;
using System;
using System.Globalization;

namespace MyTimeClassifier.UI.Components;

public class SolidPainBrushToSolidColorPaintConverter : IValueConverter
{
    public object? Convert(object? p_Value, Type p_TargetType, object? p_Parameter, CultureInfo p_Culture)
        => p_Value is SolidColorBrush l_Brush
            ? new SolidColorPaint(l_Brush.ToSKColor())
            : null;

    public object? ConvertBack(object? p_Value, Type p_TargetType, object? p_Parameter, CultureInfo p_Culture)
        => p_Value is SolidColorPaint l_Paint
            ? new SolidColorBrush(l_Paint.ToColor())
            : null;
}
