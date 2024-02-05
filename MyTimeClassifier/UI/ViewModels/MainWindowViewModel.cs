using MyTimeClassifier.Database.Entities;
using MyTimeClassifier.UI.Components;
using ReactiveUI;
using System;
using System.Windows.Input;

namespace MyTimeClassifier.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private bool m_JobIsSelected;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    private string m_SelectedJobName = string.Empty;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public MainWindowViewModel(JobRadialSelector p_CurrentJobSelector)
    {
        CurrentJobSelector = p_CurrentJobSelector;
        OnJobSelected = p_JobID =>
        {
            Console.WriteLine($"Job selected: {p_JobID}");
            CurrentJobSelector.SelectedJobID = p_JobID;
            JobIsSelected                    = p_JobID != Job.JobID.None;
            CurrentClock.Start();
        };
    }

    [Obsolete("Only for design data context.")]
    public MainWindowViewModel()
    {
        OnJobSelected      = _ => { };
        CurrentJobSelector = new JobRadialSelector();
    }

    public JobRadialSelector CurrentJobSelector { get; init; }
    public ICommand          StatsCommand       { get; } = ReactiveCommand.Create(() => Console.WriteLine("StatsCommand invoked"));
    public ICommand          HistoryCommand     { get; } = ReactiveCommand.Create(() => Console.WriteLine("HistoryCommand invoked"));
    public Clock             CurrentClock       { get; } = new();
    public bool              JobIsSelected      { get => m_JobIsSelected; set => this.RaiseAndSetIfChanged(ref m_JobIsSelected, value); }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public Action<Job.JobID> OnJobSelected { get; init; }
}
