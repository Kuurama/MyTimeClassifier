using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Threading;

namespace MyTimeClassifier.UI.Components;

public class Clock : INotifyPropertyChanged
{
    private readonly DispatcherTimer _dispatcherTimer = new();
    private readonly uint _nextCycleIntervalSeconds;

    private DateTime _nextCycle = DateTime.Now.AddHours(1);
    public EventHandler<DateTime> OnNextCycle = (_, _) => { };

    public Clock(uint nextCycleIntervalSeconds)
    {
        _dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
        _dispatcherTimer.Tick += DispatcherTimer_Tick;
        _nextCycleIntervalSeconds = nextCycleIntervalSeconds;
    }

    public DateTime StartingTime { get; set; } = DateTime.Now;

    public string CurrentTime { get; private set => SetField(ref field, value); }
        = TimeSpan.Zero.ToString(@"hh\:mm\:ss");

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Stop() => _dispatcherTimer.Stop();


    public void Start()
    {
        CurrentTime = TimeSpan.Zero.ToString(@"hh\:mm\:ss");
        StartingTime = DateTime.Now;
        _dispatcherTimer.Start();
    }

    private void DispatcherTimer_Tick(object? sender, EventArgs e)
    {
        var now = DateTime.Now;

        if (now >= _nextCycle)
        {
            /* If the next cycle is 0, then it's a one-time cycle */
            if (_nextCycleIntervalSeconds == 0)
            {
                _nextCycle = DateTime.Now.AddYears(1);
            }
            else
            {
                _nextCycle = DateTime.Now.AddSeconds(_nextCycleIntervalSeconds);
                OnNextCycle(this, now);
            }
        }

        CurrentTime = StartingTime.Subtract(now).ToString(@"hh\:mm\:ss");
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);

        return true;
    }
}