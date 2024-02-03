using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using MyTimeClassifier.Database.Entities;
using MyTimeClassifier.Utils;
using Projektanker.Icons.Avalonia;
using System;
using System.Collections.ObjectModel;

namespace MyTimeClassifier.UI.Components;

public class JobRadialSelector : Canvas
{
    public static readonly StyledProperty<int> ButtonCountProperty =
        AvaloniaProperty.Register<JobRadialSelector, int>(nameof(ButtonCount));

    public static readonly StyledProperty<Action<Job.JobID>> ButtonActionProperty =
        AvaloniaProperty.Register<JobRadialSelector, Action<Job.JobID>>(nameof(ButtonAction), p_ID => { });

    public static readonly StyledProperty<double> RadiusProperty =
        AvaloniaProperty.Register<JobRadialSelector, double>(nameof(Radius));

    public static readonly StyledProperty<bool> IsMinimalisticProperty =
        AvaloniaProperty.Register<JobRadialSelector, bool>(nameof(IsMinimalistic), true);

    public static readonly StyledProperty<uint> SpacingAngleProperty =
        AvaloniaProperty.Register<JobRadialSelector, uint>(nameof(SpacingAngle));

    public static readonly StyledProperty<ObservableCollection<Job>> JobsProperty =
        AvaloniaProperty.Register<JobRadialSelector, ObservableCollection<Job>>(nameof(Jobs), new ObservableCollection<Job>());

    public static readonly StyledProperty<float> ContentScaleProperty =
        AvaloniaProperty.Register<JobRadialSelector, float>(nameof(ContentScale), 1.0f);

    public static readonly StyledProperty<Job.JobID> HoveredJobProperty =
        AvaloniaProperty.Register<JobRadialSelector, Job.JobID>(nameof(HoveredJobID));

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public JobRadialSelector()
        => PropertyChanged += RadialSelector_PropertyChanged;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public int ButtonCount
    {
        get => GetValue(ButtonCountProperty);
        set => SetValue(ButtonCountProperty, value);
    }

    public Action<Job.JobID> ButtonAction
    {
        get => GetValue(ButtonActionProperty);
        set => SetValue(ButtonActionProperty, value);
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

    public ObservableCollection<Job> Jobs
    {
        get => GetValue(JobsProperty);
        set => SetValue(JobsProperty, value);
    }

    public float ContentScale
    {
        get => GetValue(ContentScaleProperty);
        set => SetValue(ContentScaleProperty, value);
    }

    public Job.JobID HoveredJobID
    {
        get => GetValue(HoveredJobProperty);
        set => SetValue(HoveredJobProperty, value);
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    private void RadialSelector_PropertyChanged(object? p_Sender, AvaloniaPropertyChangedEventArgs p_E)
    {
        if (Radius == 0 || Jobs.Count == 0 ||
            p_E.Property != ButtonCountProperty  && p_E.Property != ButtonActionProperty   &&
            p_E.Property != RadiusProperty       && p_E.Property != IsMinimalisticProperty &&
            p_E.Property != SpacingAngleProperty && p_E.Property != JobsProperty           &&
            p_E.Property != ContentScaleProperty)
            return;

        Render();
    }

    private void Render()
    {
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment   = VerticalAlignment.Center;
        Width               = Radius;
        Height              = Radius;

        var l_AngleStep   = 360.0 / ButtonCount;
        var l_AngleOffset = (ButtonCount & 1) == 0 ? 0 : l_AngleStep / 4;

        /// Ensure the spacing ratio is a value between 0 and 1
        var l_SpacingRatio = Math.Clamp((float)SpacingAngle / 360, 0, 1);

        for (uint l_I = 0; l_I < ButtonCount; l_I++)
        {
            var l_StartAngle = l_I * l_AngleStep + l_AngleOffset + l_SpacingRatio * l_AngleStep / 2;
            var l_SweepAngle = (1 - l_SpacingRatio) * l_AngleStep;

            var l_Button = CreateButton(l_I, l_StartAngle, l_SweepAngle);
            SetButtonPositions(l_Button, l_StartAngle, l_SweepAngle);
            AddButtonToCanvas(l_Button);
        }
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///     Converts degrees to radians.
    /// </summary>
    /// <param name="p_Degrees">The degrees to convert.</param>
    /// <returns>The converted radians.</returns>
    private static double ToRadians(double p_Degrees)
        => Math.PI * p_Degrees / 180.0;

    /// <summary>
    ///     Creates a button with a specific index, start angle, and sweep angle.
    /// </summary>
    /// <param name="p_I">The index of the button.</param>
    /// <param name="p_StartAngle">The start angle of the button.</param>
    /// <param name="p_SweepAngle">The sweep angle of the button.</param>
    /// <returns>A tuple containing the path of the button and the panel of the button.</returns>
    private (IdentifiablePath Path, StackPanel Panel) CreateButton(uint p_I, double p_StartAngle, double p_SweepAngle)
    {
        /* Make the Job button shape */
        var l_Path = new IdentifiablePath
        {
            Id              = p_I,
            Stroke          = Jobs.Count != 0 ? Jobs[(int)p_I % Jobs.Count].StrokeColor : Brushes.AntiqueWhite,
            StrokeThickness = 4,
            Fill            = Jobs.Count != 0 ? Jobs[(int)p_I % Jobs.Count].FillColor : Brushes.White,
            Data            = CreateArcPathData(p_StartAngle, p_SweepAngle, Radius / 2, 0.5d),
            Cursor          = new Cursor(StandardCursorType.Arrow)
        };

        /* Make the content of the buttons */
        /// StackPanel to hold the FontAwesomeIcon and TextBlock (to align them horizontally)
        var l_StackPanel = new StackPanel
        {
            Orientation         = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment   = VerticalAlignment.Center,
            Spacing             = 5
        };

        /// FontAwesomeIcon to display the emoji
        var l_FontAwesomeIcon = new Icon
        {
            Value = Jobs.Count != 0
                ? Jobs[(int)p_I % Jobs.Count].Emoji ?? ""
                : "",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment   = VerticalAlignment.Center,
            Foreground          = Jobs.Count != 0 ? Jobs[(int)p_I % Jobs.Count].ContentColor : Brushes.Black,
            FontSize            = 20 * ContentScale
        };

        /// TextBlock to display the job name
        var l_TextBlock = new TextBlock
        {
            Text                = Jobs.Count != 0 ? Jobs[(int)p_I % Jobs.Count].Text : "",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment   = VerticalAlignment.Center,
            Foreground          = Jobs.Count != 0 ? Jobs[(int)p_I % Jobs.Count].ContentColor : Brushes.Black,
            FontSize            = 15 * ContentScale,
            FontWeight          = FontWeight.Medium
        };

        /* Color change effect to the content */
        var l_EnterAction = (EventHandler<PointerEventArgs>)((_, _) =>
        {
            if (l_Path.Fill is not SolidColorBrush l_FillColor     ||
                l_Path.Stroke is not SolidColorBrush l_StrokeColor ||
                l_FontAwesomeIcon.Foreground is not SolidColorBrush l_ContentColor) return;

            l_Path.Fill                  = new SolidColorBrush(l_FillColor.Color.Lighten(1.2f));
            l_Path.Stroke                = new SolidColorBrush(l_StrokeColor.Color.Lighten(1.2f));
            l_FontAwesomeIcon.Foreground = new SolidColorBrush(l_ContentColor.Color.Lighten(1.2f));
            l_TextBlock.Foreground       = new SolidColorBrush(l_ContentColor.Color.Lighten(1.2f));
            l_Path.Cursor                = new Cursor(StandardCursorType.Hand);
        });

        var l_ExitAction = (EventHandler<PointerEventArgs>)((_, _) =>
        {
            if (l_Path.Fill is not SolidColorBrush l_FillColor     ||
                l_Path.Stroke is not SolidColorBrush l_StrokeColor ||
                l_FontAwesomeIcon.Foreground is not SolidColorBrush l_ContentColor) return;

            l_Path.Fill                  = new SolidColorBrush(l_FillColor.Color.Darken(1.2f));
            l_Path.Stroke                = new SolidColorBrush(l_StrokeColor.Color.Darken(1.2f));
            l_FontAwesomeIcon.Foreground = new SolidColorBrush(l_ContentColor.Color.Darken(1.2f));
            l_TextBlock.Foreground       = new SolidColorBrush(l_ContentColor.Color.Darken(1.2f));
        });

        /* Add the color change effect to the content */
        l_Path.PointerEntered            += l_EnterAction;
        l_Path.PointerExited             += l_ExitAction;
        l_FontAwesomeIcon.PointerEntered += l_EnterAction;
        l_FontAwesomeIcon.PointerExited  += l_ExitAction;
        l_TextBlock.PointerEntered       += l_EnterAction;
        l_TextBlock.PointerExited        += l_ExitAction;

        /* Add the click action to the content */
        var l_OnAction = (EventHandler<PointerPressedEventArgs>)((_, p_Args) =>
        {
            if (!p_Args.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;

            ButtonAction.Invoke(Jobs[(int)p_I % Jobs.Count].Id);
        });

        l_Path.PointerPressed            += l_OnAction;
        l_FontAwesomeIcon.PointerPressed += l_OnAction;
        l_TextBlock.PointerPressed       += l_OnAction;

        /* Add the FontAwesomeIcon and TextBlock to the StackPanel */
        l_StackPanel.Children.Add(l_FontAwesomeIcon);
        l_StackPanel.Children.Add(l_TextBlock);

        return (l_Path, l_StackPanel);
    }

    /// <summary>
    ///     Sets the position of the button based on the start angle and sweep angle.
    /// </summary>
    /// <param name="p_Button">The button to set the position for.</param>
    /// <param name="p_StartAngle">The start angle of the button.</param>
    /// <param name="p_SweepAngle">The sweep angle of the button.</param>
    private void SetButtonPositions((IdentifiablePath Path, StackPanel Panel) p_Button, double p_StartAngle, double p_SweepAngle)
    {
        /// Measure and arrange the StackPanel (So the DesiredSize is set)
        p_Button.Panel.Measure(Size.Infinity);
        p_Button.Panel.Arrange(new Rect(p_Button.Panel.DesiredSize));

        /// Calculate the center position of the button (So it's in the center of the arc shaped button)
        var l_CenterPos = CalculateCenterPosition(Radius / 2, 0.5d * Radius / 2, p_StartAngle + p_SweepAngle / 2, Width, Height);

        SetLeft(p_Button.Panel, l_CenterPos.X - p_Button.Panel.DesiredSize.Width  / 2);
        SetTop(p_Button.Panel, l_CenterPos.Y  - p_Button.Panel.DesiredSize.Height / 2);
    }

    /// <summary>
    ///     Adds the button to the canvas.
    /// </summary>
    /// <param name="p_Button">The button to add to the canvas.</param>
    private void AddButtonToCanvas((IdentifiablePath Path, StackPanel Panel) p_Button)
    {
        Children.Add(p_Button.Path);
        Children.Add(p_Button.Panel);
    }

    /// <summary>
    ///     Calculates the center position of a button based on the outer radius, inner radius, angle, canvas width, and canvas
    ///     height.
    /// </summary>
    /// <param name="p_OuterRadius">The outer radius of the button.</param>
    /// <param name="p_InnerRadius">The inner radius of the button.</param>
    /// <param name="p_Angle">The angle of the button.</param>
    /// <param name="p_CanvasWidth">The width of the canvas.</param>
    /// <param name="p_CanvasHeight">The height of the canvas.</param>
    /// <returns>The calculated center position of the button.</returns>
    private static Point CalculateCenterPosition(double p_OuterRadius, double p_InnerRadius, double p_Angle, double p_CanvasWidth, double p_CanvasHeight)
    {
        var l_AverageRadius = (p_OuterRadius + p_InnerRadius) / 2;
        var l_AngleRad      = Math.PI * p_Angle               / 180.0;
        var l_X             = l_AverageRadius                 * Math.Cos(l_AngleRad);
        var l_Y             = l_AverageRadius                 * Math.Sin(l_AngleRad);

        // Adjust for the coordinate system
        l_X += p_CanvasWidth  / 2;
        l_Y += p_CanvasHeight / 2;

        return new Point(l_X, l_Y);
    }

    /// <summary>
    ///     Creates a path geometry for an arc.
    /// </summary>
    /// <param name="p_StartAngle">The start angle of the arc.</param>
    /// <param name="p_SweepAngle">The sweep angle of the arc.</param>
    /// <param name="p_Radius">The radius of the arc.</param>
    /// <param name="p_InnerRadiusRatio">The inner radius ratio of the arc.</param>
    /// <returns>The created path geometry.</returns>
    private static PathGeometry CreateArcPathData(double p_StartAngle, double p_SweepAngle, double p_Radius, double p_InnerRadiusRatio)
    {
        var l_StartAngleRad = ToRadians(p_StartAngle);
        var l_SweepAngleRad = ToRadians(p_SweepAngle);
        var l_InnerRadius   = p_Radius * p_InnerRadiusRatio;
        var l_IsLargeArc    = p_SweepAngle > 180.0;

        var l_StartPoint = new Point(
            p_Radius * (1 + Math.Cos(l_StartAngleRad)),
            p_Radius * (1 + Math.Sin(l_StartAngleRad)));

        var l_EndPoint = new Point(
            p_Radius * (1 + Math.Cos(l_StartAngleRad + l_SweepAngleRad)),
            p_Radius * (1 + Math.Sin(l_StartAngleRad + l_SweepAngleRad)));

        var l_InnerStartPoint = new Point(
            p_Radius * (1 + p_InnerRadiusRatio * Math.Cos(l_StartAngleRad + l_SweepAngleRad)),
            p_Radius * (1 + p_InnerRadiusRatio * Math.Sin(l_StartAngleRad + l_SweepAngleRad)));

        var l_InnerEndPoint = new Point(
            p_Radius * (1 + p_InnerRadiusRatio * Math.Cos(l_StartAngleRad)),
            p_Radius * (1 + p_InnerRadiusRatio * Math.Sin(l_StartAngleRad)));

        var l_Figure = new PathFigure
        {
            StartPoint = l_StartPoint,
            IsClosed   = true,
            Segments = new PathSegments
            {
                new ArcSegment
                {
                    Point          = l_EndPoint,
                    Size           = new Size(p_Radius, p_Radius),
                    SweepDirection = SweepDirection.Clockwise,
                    IsLargeArc     = l_IsLargeArc
                },
                new LineSegment
                {
                    Point = l_InnerStartPoint
                },
                new ArcSegment
                {
                    Point          = l_InnerEndPoint,
                    Size           = new Size(l_InnerRadius, l_InnerRadius),
                    SweepDirection = SweepDirection.CounterClockwise,
                    IsLargeArc     = l_IsLargeArc
                }
            }
        };

        return new PathGeometry { Figures = new PathFigures { l_Figure } };
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    private class IdentifiablePath : Path
    {
        public uint Id { get; init; }
    }
}
