using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using LiveChartsCore.SkiaSharpView.Painting;
using MyTimeClassifier.Utils;

namespace MyTimeClassifier.UI.Components;

public class SolidPainBrushToSolidColorPaintConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is SolidColorBrush brush
            ? new SolidColorPaint(brush.ToSKColor())
            : null;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is SolidColorPaint paint
            ? new SolidColorBrush(paint.Color.ToColor())
            : null;
}