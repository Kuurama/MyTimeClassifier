using Avalonia.Media;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyTimeClassifier.Configuration;
using MyTimeClassifier.Database;
using MyTimeClassifier.Utils;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

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
    private bool              m_ApplyDateFilter;
    private DateTime?         m_EndDate        = DateTime.Today;
    private PieSeries<long>[] m_PieChartSeries = [];
    /*private StackedColumnSeries<long>[] m_StackedColumnSeries = [];*/
    private DateTime? m_StartDate;
    private Axis[]    m_XAxis = [];

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public StatisticsWindowViewModel() => UpdateData();

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public bool ApplyDateFilter
    {
        get => m_ApplyDateFilter;
        set
        {
            this.RaiseAndSetIfChanged(ref m_ApplyDateFilter, value);
            UpdateData();
        }
    }

    public DateTime? StartDate
    {
        get => m_StartDate;
        set
        {
            this.RaiseAndSetIfChanged(ref m_StartDate, value);
            UpdateData();
        }
    }

    public DateTime? EndDate
    {
        get => m_EndDate;
        set
        {
            this.RaiseAndSetIfChanged(ref m_EndDate, value);
            UpdateData();
        }
    }

    public PieSeries<long>[] PieChartSeries
    {
        get => m_PieChartSeries;
        set => this.RaiseAndSetIfChanged(ref m_PieChartSeries, value);
    }

    /*public StackedColumnSeries<long>[] StackedColumnSeries
    {
        get => m_StackedColumnSeries;
        set => this.RaiseAndSetIfChanged(ref m_StackedColumnSeries, value);
    }

    public Axis[] XAxis
    {
        get => m_XAxis;
        set => this.RaiseAndSetIfChanged(ref m_XAxis, value);
    }*/

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public void UpdateData()
    {
        using var l_DbContext = new AppDbContext();

        var l_StartDate = m_ApplyDateFilter
            ? (uint)(m_StartDate?.ToUnixTime() ?? 0)
            : 0;
        var l_EndDate = m_ApplyDateFilter
            ? (uint)(m_EndDate?.ToUnixTime() ?? uint.MaxValue)
            : uint.MaxValue;

        var l_TaskPerJobGrouping = l_DbContext.Tasks
            .Where(p_X => p_X.UnixStartTime >= l_StartDate && p_X.UnixEndTime <= l_EndDate)
            .GroupBy(p_X => p_X.JobID)
            .Select(p_X => new { JobID = p_X.Key, TotalSeconds = (uint)p_X.Sum(p_Y => p_Y.UnixEndTime - p_Y.UnixStartTime) }).ToArray();

        var l_CurrentJobs = AppConfiguration.StaticCache.Jobs;
        var l_JobValueAndCountTuples = l_TaskPerJobGrouping
            .Join(l_CurrentJobs, p_X => p_X.JobID, p_Y => p_Y.Id,
                (p_JobIDTimeSpan, p_Job) => (p_Job.Id, p_Job.Text, Paint: new SolidColorPaint((p_Job.FillColor as SolidColorBrush).ToSKColor()), p_JobIDTimeSpan.TotalSeconds)).ToArray();

        PieChartSeries = ToPiChartSeries(l_JobValueAndCountTuples);
    }

    private PieSeries<long>[] ToPiChartSeries(IEnumerable<(Guid Id, string Text, SolidColorPaint Paint, uint TotalSeconds)> p_JobNamesAndCountTuples)
    {
        return p_JobNamesAndCountTuples.Select(p_X => new PieSeries<long>
        {
            Name                  = p_X.Text,
            Values                = new[] { (long)p_X.TotalSeconds },
            Fill                  = p_X.Paint,
            ToolTipLabelFormatter = p_Value => ((UInt64)p_Value.Model).ToVeryLargeTimeString()
        }).ToArray();
    }

    private Axis[] ToAxis()
        => [new Axis { Labels = Enum.GetNames(typeof(DayOfWeek)) }];
}
