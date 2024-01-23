using Avalonia;
using Avalonia.Controls.Shapes;

namespace MyTimeClassifier.Utils;

using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;

public class RadialSelector : Canvas
{
    private readonly List<Panel> m_Elements = [];

    public RadialSelector(int p_ButtonCount, double p_Radius)
    {
        for (var l_I = 0; l_I < p_ButtonCount; l_I++)
        {
            // Create the Button
            var button = new Button
            {
                Width      = 50,
                Height     = 50,
                Background = Brushes.LightBlue,
            };

            // Create the outer Ellipse
            var outerEllipse = new Ellipse
            {
                Width           = button.Width,
                Height          = button.Height,
                Fill            = Brushes.Transparent,
                StrokeThickness = 0,
                OpacityMask = new LinearGradientBrush
                {
                    StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                    EndPoint   = new RelativePoint(0, 1, RelativeUnit.Relative),
                    GradientStops =
                    {
                        new GradientStop(Colors.Black, 0),
                        new GradientStop(Colors.Transparent, 1)
                    },
                },
            };

            // Create the inner Ellipse
            var innerEllipse = new Ellipse
            {
                Width           = button.Width  * 0.8,
                Height          = button.Height * 0.8,
                Fill            = Brushes.Transparent,
                StrokeThickness = 0,
                OpacityMask = new LinearGradientBrush
                {
                    StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                    EndPoint   = new RelativePoint(0, 1, RelativeUnit.Relative),
                    GradientStops =
                    {
                        new GradientStop(Colors.Transparent, 0),
                        new GradientStop(Colors.Black, 1)
                    },
                },
            };

            // Create a Grid to hold the Button and the two Ellipses
            var grid = new Grid();
            grid.Children.Add(button);
            grid.Children.Add(outerEllipse);
            grid.Children.Add(innerEllipse);

            // Create a separate Grid for the VisualBrush
            var maskGrid = new Grid();
            maskGrid.Children.Add(new Ellipse
            {
                Width           = outerEllipse.Width,
                Height          = outerEllipse.Height,
                Fill            = outerEllipse.Fill,
                StrokeThickness = outerEllipse.StrokeThickness,
                OpacityMask     = outerEllipse.OpacityMask,
            });
            maskGrid.Children.Add(new Ellipse
            {
                Width           = innerEllipse.Width,
                Height          = innerEllipse.Height,
                Fill            = innerEllipse.Fill,
                StrokeThickness = innerEllipse.StrokeThickness,
                OpacityMask     = innerEllipse.OpacityMask,
            });

            // Create a Rectangle with a VisualBrush as its Fill
            var maskRectangle = new Rectangle
            {
                Width  = button.Width,
                Height = button.Height,
                Fill = new VisualBrush
                {
                    Visual = maskGrid,
                },
            };

            // Create a Grid to hold the Button and the mask Rectangle
            var finalGrid = new Grid();
            finalGrid.Children.Add(button);
            finalGrid.Children.Add(maskRectangle);

            // Add the Grid to the m_Elements list
            m_Elements.Add(finalGrid);
            Children.Add(finalGrid);
        }

        LayoutUpdated += (p_S, p_E) => PositionElementsInCircle(p_Radius);
    }

    private void PositionElementsInCircle(double p_Radius)
    {
        var l_Center    = new Avalonia.Point(Bounds.Width / 2, Bounds.Height / 2);
        var l_AngleStep = 360.0 / m_Elements.Count;

        for (var l_I = 0; l_I < m_Elements.Count; l_I++)
        {
            var l_Angle = (Math.PI              / 180) * (l_I * l_AngleStep); // Convert to radians
            var l_X     = l_Center.X + p_Radius * Math.Cos(l_Angle) - m_Elements[l_I].Bounds.Width  / 2;
            var l_Y     = l_Center.Y + p_Radius * Math.Sin(l_Angle) - m_Elements[l_I].Bounds.Height / 2;

            SetLeft(m_Elements[l_I], l_X);
            SetTop(m_Elements[l_I], l_Y);
        }
    }
}
