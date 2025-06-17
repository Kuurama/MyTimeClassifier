using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MyTimeClassifier.Configuration;
using MyTimeClassifier.Database.Entities;
using MyTimeClassifier.Utils;
using ReactiveUI;

namespace MyTimeClassifier.UI.Models;

public class TaskModel : INotifyPropertyChanged
{
    private readonly Task _task;

    /// <summary>
    /// Constructor that fetches it's own job from the static cache
    /// </summary>
    /// <param name="task"></param>
    public TaskModel(Task task)
    {
        _task = task;
        Job = AppConfiguration.StaticCache.Jobs.FirstOrDefault(x => x.Id == task.JobID)
              ?? new Job { Id = Guid.Empty, Text = "Unknown" };

        OnDeleteCommand = GetDeleteCommand();
    }

    public Job Job
    {
        get;
        set
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            // Because the SelectedJob reference might still trigger the event here
            if (value is null)
                return;

            _task.JobID = value.Id;
            field = value;
            OnPropertyChanged();
        }
    }

    public bool TaskExists => _task.Id != 0;

    public DateTime EndDate
    {
        get => TimeUtils.FromUnixTime(_task.UnixEndTime);
        set
        {
            var dateTime = TimeUtils.FromUnixTime(_task.UnixEndTime);
            _task.UnixEndTime = (uint)new DateTime(
                    value.Year, value.Month, value.Day,
                    dateTime.Hour, dateTime.Minute, dateTime.Second)
                .ToUnixTime();
            OnPropertyChanged();
        }
    }

    public DateTime StartDate
    {
        get => TimeUtils.FromUnixTime(_task.UnixStartTime);
        set
        {
            var dateTime = TimeUtils.FromUnixTime(_task.UnixStartTime);
            _task.UnixStartTime = (uint)new DateTime(
                    value.Year, value.Month, value.Day,
                    dateTime.Hour, dateTime.Minute, dateTime.Second)
                .ToUnixTime();

            OnPropertyChanged();
        }
    }

    public TimeSpan StartTime
    {
        get => TimeUtils.FromUnixTime(_task.UnixStartTime).TimeOfDay;
        set
        {
            var dateTime = TimeUtils.FromUnixTime(_task.UnixStartTime);
            _task.UnixStartTime = (uint)new DateTime(
                    dateTime.Year, dateTime.Month, dateTime.Day,
                    value.Hours, value.Minutes, value.Seconds)
                .ToUnixTime();
            OnPropertyChanged();
            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(ElapsedTime));
        }
    }

    public TimeSpan EndTime
    {
        get => TimeUtils.FromUnixTime(_task.UnixEndTime).TimeOfDay;
        set
        {
            var dateTime = TimeUtils.FromUnixTime(_task.UnixEndTime);
            _task.UnixEndTime = (uint)new DateTime(
                    dateTime.Year, dateTime.Month, dateTime.Day,
                    value.Hours, value.Minutes, value.Seconds)
                .ToUnixTime();

            OnPropertyChanged();
            OnPropertyChanged(nameof(EndDate));
            OnPropertyChanged(nameof(ElapsedTime));
        }
    }

    public TimeSpan ElapsedTime
        => TimeUtils.FromUnixTime(_task.UnixEndTime) - TimeUtils.FromUnixTime(_task.UnixStartTime);

    public ICommand OnDeleteCommand { get; init; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private ReactiveCommand<Unit, Unit> GetDeleteCommand() => ReactiveCommand.Create(() =>
    {
        _task.Id = 0;
        OnPropertyChanged(nameof(TaskExists));
    });

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}