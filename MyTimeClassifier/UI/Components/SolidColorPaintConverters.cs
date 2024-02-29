using Avalonia.Data.Converters;
using Avalonia.Media;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Globalization;

namespace MyTimeClassifier.UI.Components;

public class SolidPainBrushToSolidColorPaintConverter : IValueConverter
{
    public object? Convert(object? p_Value, Type p_TargetType, object? p_Parameter, CultureInfo p_Culture)
        => p_Value is SolidColorBrush l_Brush
            ? new SolidColorPaint(ToSKColor(l_Brush))
            : null;

    public object? ConvertBack(object? p_Value, Type p_TargetType, object? p_Parameter, CultureInfo p_Culture)
        => p_Value is SolidColorPaint l_Paint
            ? new SolidColorBrush(ToColor(l_Paint))
            : null;

    private static SKColor ToSKColor(ISolidColorBrush p_Brush)
        => new(p_Brush.Color.R, p_Brush.Color.G, p_Brush.Color.B, p_Brush.Color.A);

    private static Color ToColor(Paint p_Color)
        => new(p_Color.Color.Alpha, p_Color.Color.Red, p_Color.Color.Green, p_Color.Color.Blue);
}
