using MyTimeClassifier.Configuration;
using MyTimeClassifier.Database;
using MyTimeClassifier.Database.Entities;
using MyTimeClassifier.Utils;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MyTimeClassifier.UI.Models;

public class TaskModel : INotifyPropertyChanged
{
    private readonly Task m_Task;
    private          Job  m_Job;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///     Constructor that takes a job as a reference (unlike the other constructor that fetches it's own job)
    /// </summary>
    /// <param name="p_Task"></param>
    /// <param name="p_Job"></param>
    public TaskModel(Task p_Task, ref Job p_Job)
    {
        m_Job  = p_Job;
        m_Task = p_Task;

        OnDeleteCommand = GetDeleteCommand();
    }

    /// <summary>
    ///     Constructor that fetches it's own job from the static cache
    /// </summary>
    /// <param name="p_Task"></param>
    public TaskModel(Task p_Task)
    {
        m_Task = p_Task;
        m_Job = AppConfiguration.StaticCache.Jobs.FirstOrDefault(p_Job => p_Job.Id == p_Task.JobID)
            ?? new Job { Id = Guid.Empty, Text = "Unknown" };

        OnDeleteCommand = GetDeleteCommand();
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public Job Job
    {
        get => m_Job;
        set
        {
            m_Task.JobID = value.Id;
            m_Job        = value;
            OnPropertyChanged();
        }
    }

    public uint TaskID
    {
        get => m_Task.Id;
        set
        {
            m_Task.Id = value;
            OnPropertyChanged(nameof(TaskExists));
        }
    }

    public bool TaskExists
    {
        get => m_Task.Id != 0;
    }

    public DateTime? EndDate
    {
        get => TimeUtils.FromUnixTime(m_Task.UnixEndTime);
        set
        {
            if (value is null) return;

            var l_DateTime = TimeUtils.FromUnixTime(m_Task.UnixEndTime);
            m_Task.UnixEndTime = (uint)new DateTime(value.Value.Year, value.Value.Month, value.Value.Day, l_DateTime.Hour, l_DateTime.Minute, l_DateTime.Second).ToUnixTime();
            OnPropertyChanged();
        }
    }

    public DateTime? StartDate
    {
        get => TimeUtils.FromUnixTime(m_Task.UnixStartTime);
        set
        {
            if (value is null) return;

            var l_DateTime = TimeUtils.FromUnixTime(m_Task.UnixStartTime);
            m_Task.UnixStartTime = (uint)new DateTime(value.Value.Year, value.Value.Month, value.Value.Day, l_DateTime.Hour, l_DateTime.Minute, l_DateTime.Second).ToUnixTime();
            OnPropertyChanged();
        }
    }

    public TimeSpan StartTime
    {
        get => TimeUtils.FromUnixTime(m_Task.UnixStartTime).TimeOfDay;
        set
        {
            var l_DateTime = TimeUtils.FromUnixTime(m_Task.UnixStartTime);
            m_Task.UnixStartTime = (uint)new DateTime(l_DateTime.Year, l_DateTime.Month, l_DateTime.Day, value.Hours, value.Minutes, value.Seconds).ToUnixTime();
            OnPropertyChanged();
            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(ElapsedTime));
        }
    }

    public TimeSpan EndTime
    {
        get => TimeUtils.FromUnixTime(m_Task.UnixEndTime).TimeOfDay;
        set
        {
            var l_DateTime = TimeUtils.FromUnixTime(m_Task.UnixEndTime);
            m_Task.UnixEndTime = (uint)new DateTime(l_DateTime.Year, l_DateTime.Month, l_DateTime.Day, value.Hours, value.Minutes, value.Seconds).ToUnixTime();
            OnPropertyChanged();
            OnPropertyChanged(nameof(EndDate));
            OnPropertyChanged(nameof(ElapsedTime));
        }
    }

    public TimeSpan ElapsedTime => TimeUtils.FromUnixTime(m_Task.UnixEndTime) - TimeUtils.FromUnixTime(m_Task.UnixStartTime);

    public ICommand OnDeleteCommand { get; init; }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public event PropertyChangedEventHandler? PropertyChanged;

    private ReactiveCommand<Unit, Unit> GetDeleteCommand() => ReactiveCommand.Create(() =>
    {
        TaskID = 0;
    });

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    private void OnPropertyChanged([CallerMemberName] string? p_PropertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p_PropertyName));
}
