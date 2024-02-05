using Avalonia.Threading;
using System;
using System.ComponentModel;

namespace MyTimeClassifier.UI.Components;

public class Clock : INotifyPropertyChanged
{
    private readonly DispatcherTimer m_DispatcherTimer = new();

    private DateTime               m_NextCycle = DateTime.Now.AddHours(1);
    public  EventHandler<DateTime> OnEveryHour = (p_Sender, p_E) => { };

    public Clock()
    {
        m_DispatcherTimer.Interval =  TimeSpan.FromSeconds(1);
        m_DispatcherTimer.Tick     += DispatcherTimer_Tick;
    }

    public DateTime StartingTime { get; set; } = DateTime.Now;
    public string   CurrentTime  { get; set; } = TimeSpan.Zero.ToString(@"hh\:mm\:ss");
    public string   NextCycle    { get; set; } = "New save cycle in: ";

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Stop() => m_DispatcherTimer.Stop();

    public void Start()
    {
        StartingTime = DateTime.Now;
        m_DispatcherTimer.Start();
    }

    private void DispatcherTimer_Tick(object? p_Sender, EventArgs p_E)
    {
        var l_Now = DateTime.Now;

        if (l_Now >= m_NextCycle)
        {
            m_NextCycle = DateTime.Now.AddHours(1);
            OnEveryHour(this, l_Now);
        }

        var l_TimeSpan = m_NextCycle - l_Now;
        NextCycle = string.Format(
            "Next saving cycle at {0:HH:mm:ss}(in {1}m{2}s)", m_NextCycle,
            Math.Round(l_TimeSpan.TotalMinutes) - 1,
            l_TimeSpan.Seconds
        );
        CurrentTime = StartingTime.Subtract(l_Now).ToString(@"hh\:mm\:ss");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTime)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NextCycle)));
    }
}
