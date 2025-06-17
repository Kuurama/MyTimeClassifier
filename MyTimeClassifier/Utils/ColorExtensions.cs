using System;
using Avalonia.Media;
using SkiaSharp;

namespace MyTimeClassifier.Utils;

public static class ColorExtensions
{
    public static SKColor ToSKColor(this ISolidColorBrush? brush)
        => brush is null
            ? new SKColor(0, 0, 0, 255)
            : new SKColor(brush.Color.R, brush.Color.G, brush.Color.B, brush.Color.A);

    public static Color ToColor(this SKColor color)
        => new(color.Alpha, color.Red, color.Green, color.Blue);

    public static Color Lighten(this Color color, float factor)
    {
        RGBToHSL(color.R, color.G, color.B, out var h, out var s, out var l);
        l = Math.Clamp(l * factor, 0, 1);
        RGBFromHSL(h, s, l, out var r, out var g, out var b);

        return Color.FromArgb(color.A, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
    }

    public static Color Darken(this Color color, float factor)
    {
        RGBToHSL(color.R, color.G, color.B, out var h, out var s, out var l);
        l = Math.Clamp(l / factor, 0, 1);
        RGBFromHSL(h, s, l, out var r, out var g, out var b);

        return Color.FromArgb(color.A, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
    }

    private static void RGBToHSL(byte r, byte g, byte b, out float h, out float s, out float l)
    {
        float rf = r / 255f, gf = g / 255f, bf = b / 255f;
        float max = Math.Max(rf, Math.Max(gf, bf)), min = Math.Min(rf, Math.Min(gf, bf));

        l = (max + min) / 2;

        if (Math.Abs(max - min) < 0.01f)
        {
            h = s = 0;
        }
        else
        {
            var delta = max - min;
            s = l > 0.5 ? delta / (2 - max - min) : delta / (max + min);

            if (Math.Abs(max - rf) < 0.01f)
                h = (gf - bf) / delta + (gf < bf ? 6 : 0);
            else if (Math.Abs(max - gf) < 0.01f)
                h = (bf - rf) / delta + 2;
            else
                h = (rf - gf) / delta + 4;

            h /= 6;
        }
    }

    private static void RGBFromHSL(float h, float s, float l, out float r, out float g, out float b)
    {
        if (s == 0)
        {
            r = g = b = l;
        }
        else
        {
            var q = l < 0.5 ? l * (1 + s) : l + s - l * s;
            var p = 2 * l - q;
            r = HueToRGB(p, q, h + 1 / 3f);
            g = HueToRGB(p, q, h);
            b = HueToRGB(p, q, h - 1 / 3f);
        }
    }

    private static float HueToRGB(float p, float q, float t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;

        return t switch
        {
            < 1 / 6f => p + (q - p) * 6 * t,
            < 1 / 2f => q,
            < 2 / 3f => p + (q - p) * (2 / 3f - t) * 6,
            _ => p
        };
    }
}