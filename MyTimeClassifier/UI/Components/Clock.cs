using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyTimeClassifier.UI.Components;

public class Clock : INotifyPropertyChanged
{
    private readonly DispatcherTimer m_DispatcherTimer = new();
    private readonly uint            m_NextCycleIntervalSeconds;
    private          string          m_CurrentTime = TimeSpan.Zero.ToString(@"hh\:mm\:ss");

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

    public DateTime StartingTime { get;                  set; } = DateTime.Now;
    public string   CurrentTime  { get => m_CurrentTime; private set => SetField(ref m_CurrentTime, value); }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public event PropertyChangedEventHandler? PropertyChanged;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public void Stop() => m_DispatcherTimer.Stop();


    public void Start()
    {
        CurrentTime  = TimeSpan.Zero.ToString(@"hh\:mm\:ss");
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
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    private void OnPropertyChanged([CallerMemberName] string? p_PropertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p_PropertyName));

    private bool SetField<T>(ref T p_Field, T p_Value, [CallerMemberName] string? p_PropertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(p_Field, p_Value))
            return false;

        p_Field = p_Value;
        OnPropertyChanged(p_PropertyName);

        return true;
    }
}
