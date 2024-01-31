using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
using Avalonia.Media;
using System;

namespace MyTimeClassifier.Utils;

public class RadialSelector : Canvas
{
    public static readonly StyledProperty<int> ButtonCountProperty =
        AvaloniaProperty.Register<RadialSelector, int>(nameof(ButtonCount));

    public static readonly StyledProperty<Action[]> ButtonActionsProperty =
        AvaloniaProperty.Register<RadialSelector, Action[]>(nameof(ButtonActions));

    public static readonly StyledProperty<double> RadiusProperty =
        AvaloniaProperty.Register<RadialSelector, double>(nameof(Radius));

    public static readonly StyledProperty<bool> IsMinimalisticProperty =
        AvaloniaProperty.Register<RadialSelector, bool>(nameof(IsMinimalistic), true);

    public static readonly StyledProperty<uint> SpacingAngleProperty =
        AvaloniaProperty.Register<RadialSelector, uint>(nameof(SpacingAngle));

    public static readonly StyledProperty<IBrush[]> ColorsProperty =
        AvaloniaProperty.Register<RadialSelector, IBrush[]>(nameof(Colors), Array.Empty<IBrush>());

    public RadialSelector()
        => PropertyChanged += RadialSelector_PropertyChanged;

    public int ButtonCount
    {
        get => GetValue(ButtonCountProperty);
        set => SetValue(ButtonCountProperty, value);
    }

    public Action[] ButtonActions
    {
        get => GetValue(ButtonActionsProperty);
        set => SetValue(ButtonActionsProperty, value);
    }

    public double Radius
    {
        get => GetValue(RadiusProperty);
        set => SetValue(RadiusProperty, value);
    }

    public bool IsMinimalistic
    {
        get => GetValue(IsMinimalisticProperty);
        set => SetValue(IsMinimalisticProperty, value);
    }

    public uint SpacingAngle
    {
        get => GetValue(SpacingAngleProperty);
        set => SetValue(SpacingAngleProperty, value);
    }

    public IBrush[] Colors
    {
        get => GetValue(ColorsProperty);
        set => SetValue(ColorsProperty, value);
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    private void RadialSelector_PropertyChanged(object? p_Sender, AvaloniaPropertyChangedEventArgs p_E)
    {
        if (Radius != 0 && (p_E.Property == ButtonCountProperty || p_E.Property == ButtonActionsProperty || p_E.Property == RadiusProperty || p_E.Property == IsMinimalisticProperty || p_E.Property == SpacingAngleProperty
            || p_E.Property              == ColorsProperty))
        {
            Render();
        }
    }

    private void Render()
    {
        if (ButtonActions is not null && ButtonCount != ButtonActions.Length)
            throw new ArgumentException("The number of buttons must match the number of actions.");

        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment   = VerticalAlignment.Center;
        Width               = Radius;
        Height              = Radius;

        var l_AngleStep   = 360.0 / ButtonCount;
        var l_AngleOffset = (ButtonCount & 1) == 0 ? 0 : l_AngleStep / 4;

        // Ensure the spacing ratio is a value between 0 and 1
        var l_SpacingRatio = Math.Clamp((float)SpacingAngle / 360, 0, 1);

        for (uint l_I = 0; l_I < ButtonCount; l_I++)
        {
            var l_StartAngle = l_I * l_AngleStep + l_AngleOffset + l_SpacingRatio * l_AngleStep / 2;
            var l_SweepAngle = (1 - l_SpacingRatio) * l_AngleStep;

            var l_Path = new IdentifiablePath
            {
                Id              = l_I,
                Stroke          = Colors.Length != 0 ? Colors[l_I % Colors.Length] : Brushes.AntiqueWhite,
                StrokeThickness = 4,
                Fill            = Colors.Length != 0 ? Colors[l_I % Colors.Length] : Brushes.White,
                Data            = CreateArcPathData(l_StartAngle, l_SweepAngle, Radius / 2)
            };

            l_Path.PointerPressed += (p_S, p_E) => Console.WriteLine(((IdentifiablePath)p_S!).Id);
            Children.Add(l_Path);
        }
    }

    private static PathGeometry CreateArcPathData(double p_StartAngle, double p_SweepAngle, double p_Radius, double p_InnerRadiusRatio = 0.5)
    {
        var l_StartAngleRad = Math.PI * p_StartAngle / 180.0;
        var l_SweepAngleRad = Math.PI * p_SweepAngle / 180.0;

        var l_StartX = p_Radius * (1 + Math.Cos(l_StartAngleRad));
        var l_StartY = p_Radius * (1 + Math.Sin(l_StartAngleRad));

        var l_EndX = p_Radius * (1 + Math.Cos(l_StartAngleRad + l_SweepAngleRad));
        var l_EndY = p_Radius * (1 + Math.Sin(l_StartAngleRad + l_SweepAngleRad));

        var l_InnerStartX = p_Radius * (1 + p_InnerRadiusRatio * Math.Cos(l_StartAngleRad + l_SweepAngleRad));
        var l_InnerStartY = p_Radius * (1 + p_InnerRadiusRatio * Math.Sin(l_StartAngleRad + l_SweepAngleRad));

        var l_InnerEndX = p_Radius * (1 + p_InnerRadiusRatio * Math.Cos(l_StartAngleRad));
        var l_InnerEndY = p_Radius * (1 + p_InnerRadiusRatio * Math.Sin(l_StartAngleRad));

        var l_OuterArcSegment = new ArcSegment
        {
            Point          = new Point(l_EndX, l_EndY),
            Size           = new Size(p_Radius, p_Radius),
            SweepDirection = SweepDirection.Clockwise,
            IsLargeArc     = p_SweepAngle > 180.0
        };

        var l_InnerArcSegment = new ArcSegment
        {
            Point          = new Point(l_InnerEndX, l_InnerEndY),
            Size           = new Size(p_Radius * p_InnerRadiusRatio, p_Radius * p_InnerRadiusRatio),
            SweepDirection = SweepDirection.CounterClockwise,
            IsLargeArc     = p_SweepAngle > 180.0
        };

        var l_Figure = new PathFigure
        {
            StartPoint = new Point(l_StartX, l_StartY),
            IsClosed   = true
        };

        // Add nullability check but currently l_Figure.Segments is never null because it's constructor creates a PathSegments object.
        l_Figure.Segments?.Add(l_OuterArcSegment);
        l_Figure.Segments?.Add(new LineSegment { Point = new Point(l_InnerStartX, l_InnerStartY) });
        l_Figure.Segments?.Add(l_InnerArcSegment);

        var l_Geometry = new PathGeometry();
        l_Geometry.Figures?.Add(l_Figure);

        return l_Geometry;
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    private class IdentifiablePath : Path
    {
        public uint Id { get; init; }
    }
}
