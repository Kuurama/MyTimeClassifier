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
using System.Linq;

namespace MyTimeClassifier.UI.Components;

public class JobRadialSelector : Canvas
{
    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public static readonly StyledProperty<float> InnerRadiusRatioProperty =
        AvaloniaProperty.Register<JobRadialSelector, float>(nameof(InnerRadiusRatio));

    public static readonly StyledProperty<uint> RadiusProperty =
        AvaloniaProperty.Register<JobRadialSelector, uint>(nameof(RadiusProperty));

    public static readonly StyledProperty<Action<Guid>> ButtonActionProperty =
        AvaloniaProperty.Register<JobRadialSelector, Action<Guid>>(nameof(ButtonAction), _ => { });

    public static readonly StyledProperty<bool> IsMinimalisticProperty =
        AvaloniaProperty.Register<JobRadialSelector, bool>(nameof(IsMinimalistic));

    public static readonly StyledProperty<uint> SpacingAngleProperty =
        AvaloniaProperty.Register<JobRadialSelector, uint>(nameof(SpacingAngle));

    public static readonly StyledProperty<byte> ReRenderProperty =
        AvaloniaProperty.Register<JobRadialSelector, byte>(nameof(ReRenderProperty));

    public static readonly StyledProperty<ObservableCollection<Job>> JobsProperty =
        AvaloniaProperty.Register<JobRadialSelector, ObservableCollection<Job>>(nameof(Jobs), []);

    public static readonly StyledProperty<float> ContentScaleProperty =
        AvaloniaProperty.Register<JobRadialSelector, float>(nameof(ContentScale), 1.0f);

    public static readonly StyledProperty<float> GlobalScaleProperty =
        AvaloniaProperty.Register<JobRadialSelector, float>(nameof(GlobalScale), 1.0f);

    public static readonly StyledProperty<Guid> SelectedJobIDProperty =
        AvaloniaProperty.Register<JobRadialSelector, Guid>(nameof(SelectedJobID));

    private uint m_ButtonCount;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public JobRadialSelector() => PropertyChanged += RadialSelector_PropertyChanged;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///     Percentage of the inner radius of each button.
    ///     Changes this parameter will change how big the hole in the middle of the radial selector is.
    /// </summary>
    public float InnerRadiusRatio
    {
        get => GetValue(InnerRadiusRatioProperty);
        set => SetValue(InnerRadiusRatioProperty, value);
    }

    /// <summary>
    ///     Percentage of the outer radius bound of each button.
    ///     Changes this parameter will change how big the buttons are compared to the radial selector size Radius.
    /// </summary>
    public uint Radius
    {
        get => GetValue(RadiusProperty);
        set => SetValue(RadiusProperty, value);
    }

    public Action<Guid> ButtonAction
    {
        get => GetValue(ButtonActionProperty);
        set => SetValue(ButtonActionProperty, value);
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

    public float GlobalScale
    {
        get => GetValue(GlobalScaleProperty);
        set => SetValue(GlobalScaleProperty, value);
    }

    public Guid SelectedJobID
    {
        get => GetValue(SelectedJobIDProperty);
        set => SetValue(SelectedJobIDProperty, value);
    }

    public byte ReRender
    {
        get => GetValue(ReRenderProperty);
        set => SetValue(ReRenderProperty, value);
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    private void RadialSelector_PropertyChanged(object? p_Sender, AvaloniaPropertyChangedEventArgs p_E)
    {
        /* Only re-scale the component if prompted to */
        if (p_E.Property == GlobalScaleProperty)
        {
            ApplyScaleTransform();
            return;
        }

        /* Update the button count */
        if (p_E.Property == JobsProperty || p_E.Property == ReRenderProperty)
            m_ButtonCount = (uint)Jobs.Count(p_X => p_X.Enabled);

        /* Make sure it's only re-rendering when necessary */
        if (Radius == 0 || Jobs.Count == 0 || m_ButtonCount == 0 ||
            p_E.Property != ReRenderProperty         &&
            p_E.Property != ButtonActionProperty     && p_E.Property != IsMinimalisticProperty &&
            p_E.Property != SpacingAngleProperty     && p_E.Property != JobsProperty           &&
            p_E.Property != ContentScaleProperty     && p_E.Property != SelectedJobIDProperty  &&
            p_E.Property != InnerRadiusRatioProperty && p_E.Property != RadiusProperty)
            return;

        /* Clear the visual elements and re-render */
        ClearVisual();
        Render();

        /* Apply the transform */
        ApplyScaleTransform();
    }

    /// <summary>
    ///     Make the this component scales from it's center, and apply the global scale.
    ///     Which doesn't trigger any re-render.
    /// </summary>
    private void ApplyScaleTransform()
    {
        Width                 = Radius * GlobalScale;
        Height                = Radius * GlobalScale;
        RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Absolute);
        RenderTransform       = new ScaleTransform(GlobalScale, GlobalScale);
    }

    /// <summary>
    ///     Remove all the visual elements from the Canvas's children, leaving them free for the garbage collector
    ///     Except the select circle, which is always there.
    /// </summary>
    private void ClearVisual() => Children.Clear();

    private void Render()
    {
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment   = VerticalAlignment.Center;

        var l_AngleStep   = 360.0 / m_ButtonCount;
        var l_AngleOffset = (m_ButtonCount & 1) == 0 ? 0 : l_AngleStep / 4;

        /// Ensure the spacing ratio is a value between 0 and 1
        var l_SpacingRatio = Math.Clamp((float)SpacingAngle / 360, 0, 1);

        for (uint l_I = 0; l_I < m_ButtonCount; l_I++)
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
        var l_Job = m_ButtonCount != 0 && Jobs.Count != 0 ? Jobs.Where(p_X => p_X.Enabled).ElementAt((int)p_I % (int)m_ButtonCount) : null;

        /* Make the Job button shape */
        var l_Path = new IdentifiablePath
        {
            Id                  = p_I,
            Stroke              = l_Job?.StrokeColor ?? Brushes.AntiqueWhite,
            StrokeThickness     = 4,
            Fill                = l_Job?.FillColor ?? Brushes.White,
            Data                = CreateArcPathData(0, 0, p_StartAngle, p_SweepAngle, (double)Radius / 2, InnerRadiusRatio),
            Cursor              = new Cursor(StandardCursorType.Arrow),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment   = VerticalAlignment.Center
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

        /* Make the Select circle */
        if (SelectedJobID == l_Job?.Id)
        {
            var l_SelectCirclePos = new Point(
                (double)Radius / 2 * (1 + (InnerRadiusRatio - InnerRadiusRatio / 4) * Math.Cos(ToRadians(p_StartAngle) + ToRadians(p_SweepAngle / 2))),
                (double)Radius / 2 * (1 + (InnerRadiusRatio - InnerRadiusRatio / 4) * Math.Sin(ToRadians(p_StartAngle) + ToRadians(p_SweepAngle / 2))));

            var l_SelectCircle = new Ellipse
            {
                Width           = 20,
                Height          = 20,
                Fill            = l_Job.FillColor   ?? Brushes.White,
                Stroke          = l_Job.StrokeColor ?? Brushes.AntiqueWhite,
                StrokeThickness = 3
            };

            SetLeft(l_SelectCircle, l_SelectCirclePos.X - l_SelectCircle.Width  / 2);
            SetTop(l_SelectCircle, l_SelectCirclePos.Y  - l_SelectCircle.Height / 2);

            Children.Add(l_SelectCircle);
        }

        var l_FontAwesomeIcon = null as Icon;

        try
        {
            /// FontAwesomeIcon to display the emoji
            l_FontAwesomeIcon = new Icon
            {
                Value               = l_Job?.Emoji ?? "",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center,
                Foreground          = l_Job?.ContentColor ?? Brushes.Black,
                FontSize            = 20 * ContentScale * (string.IsNullOrEmpty(l_Job?.Emoji) ? 0 : 1)
            };
        }
        catch
        {
            l_FontAwesomeIcon = new Icon
            {
                Value               = "fa-solid fa-triangle-exclamation",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center,
                Foreground          = Brushes.Red,
                FontSize            = 20 * ContentScale
            };
        }

        // TextBlock to display the job name
        // add every '\letter' to their respective format equivalent
        var l_TextBlock = new TextBlock
        {
            Text                = (l_Job?.Text ?? "").Replace("\\n", "\n"),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment   = VerticalAlignment.Center,
            Foreground          = l_Job?.ContentColor ?? Brushes.Black,
            FontSize            = 15 * ContentScale,
            FontWeight          = FontWeight.Medium,
            IsVisible           = !IsMinimalistic
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

            ButtonAction.Invoke(l_Job?.Id ?? Guid.Empty);
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
        //BUG: Apparently, on windows this is not working as expected..
        p_Button.Panel.Measure(Size.Infinity);
        p_Button.Panel.Arrange(new Rect(p_Button.Panel.DesiredSize));

        /// Calculate the center position of the button (So it's in the center of the arc shaped button)
        var l_CenterPos = CalculateCenterPosition((float)Radius / 2, InnerRadiusRatio * Radius / 2, p_StartAngle + p_SweepAngle / 2, Radius, Radius);

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
    private static PathGeometry CreateArcPathData(double p_OffsetX, double p_OffsetY, double p_StartAngle, double p_SweepAngle, double p_Radius, double p_InnerRadiusRatio)
    {
        var l_StartAngleRad = ToRadians(p_StartAngle);
        var l_SweepAngleRad = ToRadians(p_SweepAngle);
        var l_InnerRadius   = p_Radius * p_InnerRadiusRatio;
        var l_IsLargeArc    = p_SweepAngle > 180.0;

        var l_StartPoint = new Point(
            p_OffsetX + p_Radius * (1 + Math.Cos(l_StartAngleRad)),
            p_OffsetX + p_Radius * (1 + Math.Sin(l_StartAngleRad)));

        var l_EndPoint = new Point(
            p_OffsetX + p_Radius * (1 + Math.Cos(l_StartAngleRad + l_SweepAngleRad)),
            p_OffsetX + p_Radius * (1 + Math.Sin(l_StartAngleRad + l_SweepAngleRad)));

        var l_InnerStartPoint = new Point(
            p_OffsetX + p_Radius * (1 + p_InnerRadiusRatio * Math.Cos(l_StartAngleRad + l_SweepAngleRad)),
            p_OffsetX + p_Radius * (1 + p_InnerRadiusRatio * Math.Sin(l_StartAngleRad + l_SweepAngleRad)));

        var l_InnerEndPoint = new Point(
            p_OffsetX + p_Radius * (1 + p_InnerRadiusRatio * Math.Cos(l_StartAngleRad)),
            p_OffsetX + p_Radius * (1 + p_InnerRadiusRatio * Math.Sin(l_StartAngleRad)));

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
