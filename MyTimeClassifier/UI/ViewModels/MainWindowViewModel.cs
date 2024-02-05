using MyTimeClassifier.Database.Entities;
using ReactiveUI;
using System;
using System.Windows.Input;

namespace MyTimeClassifier.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel(Action<Job.JobID> p_JobSelectedCallback)
    {
        OnJobSelected = (p_ID =>
        {
            Console.WriteLine("OnJobSelected invoked with ID: " + p_ID);
        }) + p_JobSelectedCallback;
    }

    [Obsolete("Only for design data context.")]
    public MainWindowViewModel()
    {
        OnJobSelected = _ => { };
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public ICommand StatsCommand   { get; } = ReactiveCommand.Create(() => Console.WriteLine("StatsCommand invoked"));
    public ICommand HistoryCommand { get; } = ReactiveCommand.Create(() => Console.WriteLine("HistoryCommand invoked"));

    public Action<Job.JobID> OnJobSelected { get; set; }
}
