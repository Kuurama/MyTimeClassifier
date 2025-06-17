using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia.Media;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView.Painting;
using MyTimeClassifier.Configuration;
using MyTimeClassifier.Database;
using MyTimeClassifier.Utils;
using ReactiveUI;

namespace MyTimeClassifier.UI.ViewModels;

public class StatisticsWindowViewModel : ViewModelBase, IDisposable
{
    private static PieChart _pie = null!;

    public StatisticsWindowViewModel(PieChart pie)
    {
        UpdateData();
        _pie = pie;
        PropertyChanged += UpdatePieGraphIfSeriesChanged;
    }

    public bool ApplyDateFilter { get; set => this.RaiseAndSetIfChanged(ref field, value); }

    public DateTime? StartDate { get; set => this.RaiseAndSetIfChanged(ref field, value); }

    public DateTime? EndDate { get; set => this.RaiseAndSetIfChanged(ref field, value); } = DateTime.Today;

    public PieSeries<long>[] PieChartSeries
    {
        get;
        set
        {
            field = value;
            this.RaisePropertyChanged();
        }
    } = [];

    public void Dispose()
    {
        PropertyChanged -= UpdatePieGraphIfSeriesChanged;
        GC.SuppressFinalize(this);
    }

    private void UpdatePieGraphIfSeriesChanged(object? _, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == nameof(PieChartSeries)) return;

        UpdateData();
        // Force rerendering the Pie chart (because it doesn't update unless we over it, even if AutoUpdate is enabled)
        _pie.CoreChart.Update(new ChartUpdateParams { IsAutomaticUpdate = false, Throttling = false });
    }

    private void UpdateData()
    {
        using var dbContext = new AppDbContext();

        var startDate = ApplyDateFilter
            ? (uint)(StartDate?.ToUnixTime() ?? 0)
            : 0;
        var endDate = ApplyDateFilter
            ? (uint)(EndDate?.ToUnixTime() ?? uint.MaxValue)
            : uint.MaxValue;

        var taskPerJobGrouping = dbContext.Tasks
            .Where(x => x.UnixStartTime >= startDate && x.UnixEndTime <= endDate)
            .GroupBy(x => x.JobID)
            .Select(x => new
                { JobID = x.Key, TotalSeconds = (uint)x.Sum(y => y.UnixEndTime - y.UnixStartTime) })
            .ToArray();

        var currentJobs = AppConfiguration.StaticCache.Jobs;
        var jobValueAndCountTuples = taskPerJobGrouping
            .Join(currentJobs, x => x.JobID, y => y.Id,
                (jobIDTimeSpan, job) => (job.Id, job.Text,
                    Paint: new SolidColorPaint((job.FillColor as SolidColorBrush).ToSKColor()),
                    jobIDTimeSpan.TotalSeconds))
            .ToArray();

        PieChartSeries = ToPiChartSeries(jobValueAndCountTuples);
    }

    private PieSeries<long>[] ToPiChartSeries(
        IEnumerable<(Guid Id, string Text, SolidColorPaint Paint, uint TotalSeconds)> jobNamesAndCountTuples)
        => jobNamesAndCountTuples.Select(x => new PieSeries<long>
        {
            Name = x.Text.Replace("\\n", " "),
            Values = [x.TotalSeconds],
            Fill = x.Paint,
            ToolTipLabelFormatter = value => ((ulong)value.Model).ToVeryLargeTimeString()
        }).ToArray();

    private Axis[] ToAxis()
        => [new() { Labels = Enum.GetNames(typeof(DayOfWeek)) }];
}