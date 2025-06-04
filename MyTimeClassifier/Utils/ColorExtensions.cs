using Avalonia.Media;
using SkiaSharp;
using System;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Painting;

namespace MyTimeClassifier.Utils;

public static class ColorExtensions
{
    public static SKColor ToSKColor(this ISolidColorBrush? p_Brush)
        => p_Brush is null ? new SKColor(0, 0, 0, 255) : new SKColor(p_Brush.Color.R, p_Brush.Color.G, p_Brush.Color.B, p_Brush.Color.A);

    public static Color ToColor(this SKColor p_Color)
        => new(p_Color.Alpha, p_Color.Red, p_Color.Green, p_Color.Blue);

    public static Color Lighten(this Color p_Color, float p_Factor)
    {
        RGBToHSL(p_Color.R, p_Color.G, p_Color.B, out var l_H, out var l_S, out var l_L);
        l_L = Math.Clamp(l_L * p_Factor, 0, 1);
        RGBFromHSL(l_H, l_S, l_L, out var l_R, out var l_G, out var l_B);

        return Color.FromArgb(p_Color.A, (byte)(l_R * 255), (byte)(l_G * 255), (byte)(l_B * 255));
    }

    public static Color Darken(this Color p_Color, float p_Factor)
    {
        RGBToHSL(p_Color.R, p_Color.G, p_Color.B, out var l_H, out var l_S, out var l_L);
        l_L = Math.Clamp(l_L / p_Factor, 0, 1);
        RGBFromHSL(l_H, l_S, l_L, out var l_R, out var l_G, out var b);

        return Color.FromArgb(p_Color.A, (byte)(l_R * 255), (byte)(l_G * 255), (byte)(b * 255));
    }

    private static void RGBToHSL(byte p_R, byte p_G, byte p_B, out float p_H, out float p_S, out float p_L)
    {
        float l_Rf  = p_R / 255f,                           l_Gf  = p_G / 255f, l_Bf = p_B / 255f;
        float l_Max = Math.Max(l_Rf, Math.Max(l_Gf, l_Bf)), l_Min = Math.Min(l_Rf, Math.Min(l_Gf, l_Bf));

        p_L = (l_Max + l_Min) / 2;

        if (Math.Abs(l_Max - l_Min) < 0.01f)
        {
            p_H = p_S = 0;
        }
        else
        {
            var l_D = l_Max - l_Min;
            p_S = p_L > 0.5 ? l_D / (2 - l_Max - l_Min) : l_D / (l_Max + l_Min);

            if (Math.Abs(l_Max - l_Rf) < 0.01f)
                p_H = (l_Gf - l_Bf) / l_D + (l_Gf < l_Bf ? 6 : 0);
            else if (Math.Abs(l_Max - l_Gf) < 0.01f)
                p_H = (l_Bf - l_Rf) / l_D + 2;
            else
                p_H = (l_Rf - l_Gf) / l_D + 4;

            p_H /= 6;
        }
    }

    private static void RGBFromHSL(float p_H, float p_S, float p_L, out float p_R, out float p_G, out float p_B)
    {
        if (p_S == 0)
            p_R = p_G = p_B = p_L;
        else
        {
            var l_Q = p_L < 0.5 ? p_L * (1 + p_S) : p_L + p_S - p_L * p_S;
            var l_P = 2               * p_L - l_Q;
            p_R = HueToRGB(l_P, l_Q, p_H + 1 / 3f);
            p_G = HueToRGB(l_P, l_Q, p_H);
            p_B = HueToRGB(l_P, l_Q, p_H - 1 / 3f);
        }
    }

    private static float HueToRGB(float p_P, float p_Q, float p_T)
    {
        if (p_T < 0) p_T += 1;
        if (p_T > 1) p_T -= 1;
        return p_T switch
        {
            < 1 / 6f => p_P + (p_Q - p_P) * 6 * p_T,
            < 1 / 2f => p_Q,
            < 2 / 3f => p_P + (p_Q - p_P) * (2 / 3f - p_T) * 6,
            _        => p_P
        };
    }
}
