using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace MyTimeClassifier.UI.ViewModels;

/// <summary>
///     This class will be in charge of updating the Configuration singleton and updating the UI accordingly
/// </summary>
public class StatisticsWindowViewModel : ViewModelBase
{
    /*private const */

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /*private */

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public ISeries[] PieSeries { get; set; } =
    [
        new PieSeries<double>
        {
            Name   = "No",
            Values = new double[] { 2 },
            Fill   = new SolidColorPaint(SKColors.Blue)
        },
        new PieSeries<double>
        {
            Name   = "No in red",
            Values = new double[] { 4 },
            Fill   = new SolidColorPaint(SKColors.Red)
        }
    ];

    public ISeries[] LineSeries { get; set; } =
    [
        new LineSeries<double>
        {
            Values = new double[] { 2, 1, 3, 5, 3, 4, 6 },
            Fill   = null
        }
    ];

    /*public Chart[] Charts { get; set; } =
    [
        new BarChart { Entries         = CreateTestEntries() },
        new PointChart { Entries       = CreateTestEntries() },
        new LineChart { Entries        = CreateTestEntries() },
        new DonutChart { Entries       = CreateTestEntries() },
        new RadialGaugeChart { Entries = CreateTestEntries() },
        new RadarChart { Entries       = CreateTestEntries() }
    ];*/

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /*public void () */
}
