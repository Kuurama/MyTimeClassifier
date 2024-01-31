using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
using Avalonia.Media;
using System;

namespace MyTimeClassifier.Utils;

public class RadialSelector : Canvas
{
    public RadialSelector(int p_ButtonCount, Action[]? p_ButtonActions, double p_Radius, bool p_IsMinimalistic = true, float p_SpacingRatio = 0)
    {
        if (p_ButtonActions is not null && p_ButtonCount != p_ButtonActions.Length)
            throw new ArgumentException("The number of buttons must match the number of actions.");

        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment   = VerticalAlignment.Center;
        Width               = p_Radius;
        Height              = p_Radius;

        var l_AngleStep   = 360.0       / p_ButtonCount;
        var l_AngleOffset = l_AngleStep / p_ButtonCount;

        for (uint l_I = 0; l_I < p_ButtonCount; l_I++)
        {
            var l_StartAngle = l_I * l_AngleStep;

            if (p_ButtonCount % 2 != 0)
            {
                l_StartAngle += l_AngleStep / 2;
            }

            var l_Path = new IdentifiablePath
            {
                Id              = l_I,
                Stroke          = Brushes.Gray,
                StrokeThickness = 4,
                Fill            = Brushes.WhiteSmoke,
                Data            = CreateArcPathData(l_StartAngle, l_AngleStep - l_AngleOffset, p_Radius / 2)
            };

            l_Path.PointerPressed += (p_S, p_E) => Console.WriteLine(((IdentifiablePath)p_S!).Id);
            Children.Add(l_Path);
        }
    }

    private static Geometry CreateArcPathData(double p_StartAngle, double p_SweepAngle, double p_Radius, double p_InnerRadiusRatio = 0.5)
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
        l_Figure.Segments?.Add(l_OuterArcSegment);
        l_Figure.Segments?.Add(new LineSegment { Point = new Point(l_InnerStartX, l_InnerStartY) });
        l_Figure.Segments?.Add(l_InnerArcSegment);

        var l_Geometry = new PathGeometry();
        l_Geometry.Figures?.Add(l_Figure);

        return l_Geometry;
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public class IdentifiablePath : Path
    {
        public uint Id { get; init; }
    }
}
