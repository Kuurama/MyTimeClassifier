using Avalonia.Threading;
using System;
using System.ComponentModel;

namespace MyTimeClassifier.UI.Components;

public class Clock : INotifyPropertyChanged
{
    private readonly DispatcherTimer m_DispatcherTimer = new();
    private readonly uint            m_NextCycleIntervalSeconds;

    private DateTime               m_NextCycle = DateTime.Now.AddHours(1);
    public  EventHandler<DateTime> OnNextCycle = (_, _) => { };

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public Clock(uint p_NextCycleIntervalSeconds)
    {
        m_DispatcherTimer.Interval =  TimeSpan.FromSeconds(1);
        m_DispatcherTimer.Tick     += DispatcherTimer_Tick;
        m_NextCycleIntervalSeconds =  p_NextCycleIntervalSeconds;
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public DateTime StartingTime { get; set; } = DateTime.Now;
    public string   CurrentTime  { get; set; } = TimeSpan.Zero.ToString(@"hh\:mm\:ss");

    public event PropertyChangedEventHandler? PropertyChanged;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

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
            /* If the next cycle is 0, then it's a one-time cycle */
            if (m_NextCycleIntervalSeconds == 0)
                m_NextCycle = DateTime.Now.AddYears(1);
            else
            {
                m_NextCycle = DateTime.Now.AddSeconds(m_NextCycleIntervalSeconds);
                OnNextCycle(this, l_Now);
            }
        }

        CurrentTime = StartingTime.Subtract(l_Now).ToString(@"hh\:mm\:ss");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTime)));
    }
}
