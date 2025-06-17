using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using MyTimeClassifier.Database.Entities;
using MyTimeClassifier.Utils;
using Projektanker.Icons.Avalonia;

namespace MyTimeClassifier.UI.Components;

public class JobRadialSelector : Canvas
{
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

    public JobRadialSelector() => PropertyChanged += RadialSelector_PropertyChanged;

    /// <summary>
    /// Percentage of the inner radius of each button.
    /// Changes this parameter will change how big the hole in the middle of the radial selector is.
    /// </summary>
    public float InnerRadiusRatio
    {
        get => GetValue(InnerRadiusRatioProperty);
        set => SetValue(InnerRadiusRatioProperty, value);
    }

    /// <summary>
    /// Percentage of the outer radius bound of each button.
    /// Changes this parameter will change how big the buttons are compared to the radial selector size Radius.
    /// </summary>
    public uint Radius { get => GetValue(RadiusProperty); set => SetValue(RadiusProperty, value); }

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

    public uint SpacingAngle { get => GetValue(SpacingAngleProperty); set => SetValue(SpacingAngleProperty, value); }

    public ObservableCollection<Job> Jobs { get => GetValue(JobsProperty); set => SetValue(JobsProperty, value); }

    public float ContentScale { get => GetValue(ContentScaleProperty); set => SetValue(ContentScaleProperty, value); }

    public float GlobalScale { get => GetValue(GlobalScaleProperty); set => SetValue(GlobalScaleProperty, value); }

    public Guid SelectedJobID { get => GetValue(SelectedJobIDProperty); set => SetValue(SelectedJobIDProperty, value); }

    public byte ReRender { get => GetValue(ReRenderProperty); set => SetValue(ReRenderProperty, value); }

    private class IdentifiablePath : Path
    {
        public uint Id { get; init; }
    }

    private void RadialSelector_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        /* Only re-scale the component if prompted to */
        if (e.Property == GlobalScaleProperty)
        {
            ApplyScaleTransform();
            return;
        }

        /* Update the button count */
        if (e.Property == JobsProperty || e.Property == ReRenderProperty)
            m_ButtonCount = (uint)Jobs.Count(x => x.Enabled);

        /* Make sure it's only re-rendering when necessary */
        if (Radius == 0 || Jobs.Count == 0 || m_ButtonCount == 0 ||
            e.Property != ReRenderProperty &&
            e.Property != ButtonActionProperty && e.Property != IsMinimalisticProperty &&
            e.Property != SpacingAngleProperty && e.Property != JobsProperty &&
            e.Property != ContentScaleProperty && e.Property != SelectedJobIDProperty &&
            e.Property != InnerRadiusRatioProperty && e.Property != RadiusProperty)
            return;

        /* Clear the visual elements and re-render */
        ClearVisual();
        Render();

        /* Apply the transform */
        ApplyScaleTransform();
    }

    /// <summary>
    /// Make the this component scales from it's center, and apply the global scale.
    /// Which doesn't trigger any re-render.
    /// </summary>
    private void ApplyScaleTransform()
    {
        Width = Radius * GlobalScale;
        Height = Radius * GlobalScale;
        RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Absolute);
        RenderTransform = new ScaleTransform(GlobalScale, GlobalScale);
    }

    /// <summary>
    /// Remove all the visual elements from the Canvas's children, leaving them free for the garbage collector
    /// Except the select circle, which is always there.
    /// </summary>
    private void ClearVisual() => Children.Clear();

    private void Render()
    {
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;

        var angleStep = 360.0 / m_ButtonCount;
        var angleOffset = (m_ButtonCount & 1) == 0 ? 0 : angleStep / 4;

        // Ensure the spacing ratio is a value between 0 and 1
        var spacingRatio = Math.Clamp((float)SpacingAngle / 360, 0, 1);

        for (uint i = 0; i < m_ButtonCount; i++)
        {
            var startAngle = i * angleStep + angleOffset + spacingRatio * angleStep / 2;
            var sweepAngle = (1 - spacingRatio) * angleStep;

            var button = CreateButton(i, startAngle, sweepAngle);
            SetButtonPositions(button, startAngle, sweepAngle);
            AddButtonToCanvas(button);
        }
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Converts degrees to radians.
    /// </summary>
    /// <param name="degrees">The degrees to convert.</param>
    /// <returns>The converted radians.</returns>
    private static double ToRadians(double degrees)
        => Math.PI * degrees / 180.0;

    /// <summary>
    /// Creates a button with a specific index, start angle, and sweep angle.
    /// </summary>
    /// <param name="i">The index of the button.</param>
    /// <param name="startAngle">The start angle of the button.</param>
    /// <param name="sweepAngle">The sweep angle of the button.</param>
    /// <returns>A tuple containing the path of the button and the panel of the button.</returns>
    private (IdentifiablePath Path, StackPanel Panel) CreateButton(uint i, double startAngle, double sweepAngle)
    {
        var job = m_ButtonCount != 0 && Jobs.Count != 0
            ? Jobs.Where(x => x.Enabled).ElementAt((int)i % (int)m_ButtonCount)
            : null;

        /* Make the Job button shape */
        var path = new IdentifiablePath
        {
            Id = i,
            Stroke = job?.StrokeColor ?? Brushes.AntiqueWhite,
            StrokeThickness = 4,
            Fill = job?.FillColor ?? Brushes.White,
            Data = CreateArcPathData(0, 0, startAngle, sweepAngle, (double)Radius / 2, InnerRadiusRatio),
            Cursor = new Cursor(StandardCursorType.Arrow),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        /* Make the content of the buttons */
        // StackPanel to hold the FontAwesomeIcon and TextBlock (to align them horizontally)
        var stackPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Spacing = 5
        };

        /* Make the Select circle */
        if (SelectedJobID == job?.Id)
        {
            var selectCirclePos = new Point(
                (double)Radius / 2 * (1 + (InnerRadiusRatio - InnerRadiusRatio / 4) *
                    Math.Cos(ToRadians(startAngle) + ToRadians(sweepAngle / 2))),
                (double)Radius / 2 * (1 + (InnerRadiusRatio - InnerRadiusRatio / 4) *
                    Math.Sin(ToRadians(startAngle) + ToRadians(sweepAngle / 2))));

            var selectCircle = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = job.FillColor ?? Brushes.White,
                Stroke = job.StrokeColor ?? Brushes.AntiqueWhite,
                StrokeThickness = 3
            };

            SetLeft(selectCircle, selectCirclePos.X - selectCircle.Width / 2);
            SetTop(selectCircle, selectCirclePos.Y - selectCircle.Height / 2);

            Children.Add(selectCircle);
        }

        var fontAwesomeIcon = null as Icon;

        try
        {
            // FontAwesomeIcon to display the emoji
            fontAwesomeIcon = new Icon
            {
                Value = job?.Emoji ?? "",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = job?.ContentColor ?? Brushes.Black,
                FontSize = 20 * ContentScale * (string.IsNullOrEmpty(job?.Emoji) ? 0 : 1)
            };
        }
        catch
        {
            fontAwesomeIcon = new Icon
            {
                Value = "fa-solid fa-triangle-exclamation",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = Brushes.Red,
                FontSize = 20 * ContentScale
            };
        }

        // TextBlock to display the job name
        // add every '\letter' to their respective format equivalent
        var textBlock = new TextBlock
        {
            Text = (job?.Text ?? "").Replace("\\n", "\n"),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = job?.ContentColor ?? Brushes.Black,
            FontSize = 15 * ContentScale,
            FontWeight = FontWeight.Medium,
            IsVisible = !IsMinimalistic
        };

        /* Color change effect to the content */
        var enterAction = (EventHandler<PointerEventArgs>)((_, _) =>
        {
            if (path.Fill is not SolidColorBrush fillColor ||
                path.Stroke is not SolidColorBrush strokeColor ||
                fontAwesomeIcon.Foreground is not SolidColorBrush contentColor) return;

            path.Fill = new SolidColorBrush(fillColor.Color.Lighten(1.2f));
            path.Stroke = new SolidColorBrush(strokeColor.Color.Lighten(1.2f));
            fontAwesomeIcon.Foreground = new SolidColorBrush(contentColor.Color.Lighten(1.2f));
            textBlock.Foreground = new SolidColorBrush(contentColor.Color.Lighten(1.2f));
            path.Cursor = new Cursor(StandardCursorType.Hand);
        });

        var exitAction = (EventHandler<PointerEventArgs>)((_, _) =>
        {
            if (path.Fill is not SolidColorBrush fillColor ||
                path.Stroke is not SolidColorBrush strokeColor ||
                fontAwesomeIcon.Foreground is not SolidColorBrush contentColor) return;

            path.Fill = new SolidColorBrush(fillColor.Color.Darken(1.2f));
            path.Stroke = new SolidColorBrush(strokeColor.Color.Darken(1.2f));
            fontAwesomeIcon.Foreground = new SolidColorBrush(contentColor.Color.Darken(1.2f));
            textBlock.Foreground = new SolidColorBrush(contentColor.Color.Darken(1.2f));
        });

        /* Add the color change effect to the content */
        path.PointerEntered += enterAction;
        path.PointerExited += exitAction;
        fontAwesomeIcon.PointerEntered += enterAction;
        fontAwesomeIcon.PointerExited += exitAction;
        textBlock.PointerEntered += enterAction;
        textBlock.PointerExited += exitAction;

        /* Add the click action to the content */
        var onAction = (EventHandler<PointerPressedEventArgs>)((_, args) =>
        {
            if (!args.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;

            ButtonAction.Invoke(job?.Id ?? Guid.Empty);
        });

        path.PointerPressed += onAction;
        fontAwesomeIcon.PointerPressed += onAction;
        textBlock.PointerPressed += onAction;

        /* Add the FontAwesomeIcon and TextBlock to the StackPanel */
        stackPanel.Children.Add(fontAwesomeIcon);
        stackPanel.Children.Add(textBlock);

        return (path, stackPanel);
    }

    /// <summary>
    /// Sets the position of the button based on the start angle and sweep angle.
    /// </summary>
    /// <param name="button">The button to set the position for.</param>
    /// <param name="startAngle">The start angle of the button.</param>
    /// <param name="sweepAngle">The sweep angle of the button.</param>
    private void SetButtonPositions(
        (IdentifiablePath Path, StackPanel Panel) button, double startAngle, double sweepAngle)
    {
        // Measure and arrange the StackPanel (So the DesiredSize is set)
        //BUG: Apparently, on windows this is not working as expected..
        button.Panel.Measure(Size.Infinity);
        button.Panel.Arrange(new Rect(button.Panel.DesiredSize));

        // Calculate the center position of the button (So it's in the center of the arc shaped button)
        var centerPos = CalculateCenterPosition((float)Radius / 2, InnerRadiusRatio * Radius / 2,
            startAngle + sweepAngle / 2, Radius, Radius);

        SetLeft(button.Panel, centerPos.X - button.Panel.DesiredSize.Width / 2);
        SetTop(button.Panel, centerPos.Y - button.Panel.DesiredSize.Height / 2);
    }

    /// <summary>
    /// Adds the button to the canvas.
    /// </summary>
    /// <param name="button">The button to add to the canvas.</param>
    private void AddButtonToCanvas((IdentifiablePath Path, StackPanel Panel) button)
    {
        Children.Add(button.Path);
        Children.Add(button.Panel);
    }

    /// <summary>
    /// Calculates the center position of a button based on the outer radius, inner radius, angle, canvas width, and canvas
    /// height.
    /// </summary>
    /// <param name="outerRadius">The outer radius of the button.</param>
    /// <param name="innerRadius">The inner radius of the button.</param>
    /// <param name="angle">The angle of the button.</param>
    /// <param name="canvasWidth">The width of the canvas.</param>
    /// <param name="canvasHeight">The height of the canvas.</param>
    /// <returns>The calculated center position of the button.</returns>
    private static Point CalculateCenterPosition(
        double outerRadius, double innerRadius, double angle,
        double canvasWidth, double canvasHeight)
    {
        var averageRadius = (outerRadius + innerRadius) / 2;
        var angleRad = Math.PI * angle / 180.0;
        var x = averageRadius * Math.Cos(angleRad);
        var y = averageRadius * Math.Sin(angleRad);

        // Adjust for the coordinate system
        x += canvasWidth / 2;
        y += canvasHeight / 2;

        return new Point(x, y);
    }

    /// <summary>
    /// Creates a path geometry for an arc.
    /// </summary>
    /// <param name="offsetX">The X offset for the arc.</param>
    /// <param name="offsetY">The Y offset for the arc.</param>
    /// <param name="startAngle">The start angle of the arc.</param>
    /// <param name="sweepAngle">The sweep angle of the arc.</param>
    /// <param name="radius">The radius of the arc.</param>
    /// <param name="innerRadiusRatio">The inner radius ratio of the arc.</param>
    /// <returns>The created path geometry.</returns>
    private static PathGeometry CreateArcPathData(
        double offsetX, double offsetY, double startAngle,
        double sweepAngle, double radius, double innerRadiusRatio)
    {
        var startAngleRad = ToRadians(startAngle);
        var sweepAngleRad = ToRadians(sweepAngle);
        var innerRadius = radius * innerRadiusRatio;
        var isLargeArc = sweepAngle > 180.0;

        var startPoint = new Point(
            offsetX + radius * (1 + Math.Cos(startAngleRad)),
            offsetY + radius * (1 + Math.Sin(startAngleRad)));

        var endPoint = new Point(
            offsetX + radius * (1 + Math.Cos(startAngleRad + sweepAngleRad)),
            offsetY + radius * (1 + Math.Sin(startAngleRad + sweepAngleRad)));

        var innerStartPoint = new Point(
            offsetX + radius * (1 + innerRadiusRatio * Math.Cos(startAngleRad + sweepAngleRad)),
            offsetY + radius * (1 + innerRadiusRatio * Math.Sin(startAngleRad + sweepAngleRad)));

        var innerEndPoint = new Point(
            offsetX + radius * (1 + innerRadiusRatio * Math.Cos(startAngleRad)),
            offsetY + radius * (1 + innerRadiusRatio * Math.Sin(startAngleRad)));

        var figure = new PathFigure
        {
            StartPoint = startPoint,
            IsClosed = true,
            Segments =
            [
                new ArcSegment
                {
                    Point = endPoint,
                    Size = new Size(radius, radius),
                    SweepDirection = SweepDirection.Clockwise,
                    IsLargeArc = isLargeArc
                },

                new LineSegment
                {
                    Point = innerStartPoint
                },

                new ArcSegment
                {
                    Point = innerEndPoint,
                    Size = new Size(innerRadius, innerRadius),
                    SweepDirection = SweepDirection.CounterClockwise,
                    IsLargeArc = isLargeArc
                }
            ]
        };

        return new PathGeometry { Figures = [figure] };
    }
}