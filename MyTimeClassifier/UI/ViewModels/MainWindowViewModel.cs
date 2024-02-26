using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using MyTimeClassifier.Configuration;
using MyTimeClassifier.Database;
using MyTimeClassifier.UI.Components;
using MyTimeClassifier.Utils;
using ReactiveUI;
using System;
using System.Windows.Input;

namespace MyTimeClassifier.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private bool m_JobIsSelected;

    public MainWindowViewModel(JobRadialSelector p_CurrentJobSelector)
    {
        /* Ensure that the task will be stored when the application is stored or crash */
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime l_Desktop)
            l_Desktop.ShutdownRequested += (_, _) => StoreCurrentTaskIfNecessary();

        CurrentClock = new Clock(DefaultConfiguration.AUTO_SAVE_INTERVAL_SECONDS)
        {
            /* Auto save at interval */
            OnNextCycle = (_, _) =>
            {
                AppConfiguration.SaveConfiguration();
                // Not yet storing on auto-save because it might add a lot of noise to the database.
            }
        };
        CurrentJobSelector = p_CurrentJobSelector;
        OnJobSelected = p_JobID =>
        {
            if (p_JobID == Guid.Empty) throw new ArgumentException("JobID.None is not a valid job.");

            /* Make sure to stop the current job if it's the same as the one selected. */
            if (CurrentJobSelector.SelectedJobID != Guid.Empty)
                StopCommand.Execute(null);

            Console.WriteLine($"Job selected: {p_JobID}");
            CurrentJobSelector.SelectedJobID = p_JobID;
            JobIsSelected                    = p_JobID != Guid.Empty;
            CurrentClock.Start();
        };
        /* Save the current job and then stop the clock and unselect the job. */
        StopCommand = ReactiveCommand.Create(() =>
        {
            Console.WriteLine("StopCommand invoked");
            Console.WriteLine($"Job deselected was {CurrentJobSelector.SelectedJobID}");

            CurrentClock.Stop();
            StoreCurrentTaskIfNecessary();

            /* Reset the job selector (after it's been saved if necessary) */
            CurrentJobSelector.SelectedJobID = Guid.Empty;
            JobIsSelected                    = false;
        });
    }

    [Obsolete("Only for design data context.")]
    public MainWindowViewModel()
    {
        OnJobSelected      = _ => { };
        CurrentJobSelector = new JobRadialSelector();
        JobIsSelected      = true;
        CurrentClock       = new Clock(0);
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public ICommand StopCommand { get; init; }

    public JobRadialSelector CurrentJobSelector { get; init; }
    public Clock             CurrentClock       { get; init; }

    public bool JobIsSelected { get => m_JobIsSelected; set => this.RaiseAndSetIfChanged(ref m_JobIsSelected, value); }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public Action<Guid> OnJobSelected { get; init; }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///     Stores the current task if a job is selected, and if the task is longer than 5 seconds.
    /// </summary>
    private void StoreCurrentTaskIfNecessary()
    {
        if (!JobIsSelected) return;

        var l_StartingTimeUnix = CurrentClock.StartingTime.ToUnixTime();
        var l_CurrentUnixTime  = TimeUtils.UnixTimeNow();

        /* Don't store Tasks that are less than 5 seconds. */
        if (l_CurrentUnixTime - l_StartingTimeUnix < 5)
        {
            Console.WriteLine("Shorter than 5 seconds, not storing.");
            return;
        }

        Console.WriteLine($"Storing task from {l_StartingTimeUnix} to {l_CurrentUnixTime}");
        /* Store the task in the database. */
        StaticRepo.StoreTask(CurrentJobSelector.SelectedJobID, (uint)l_StartingTimeUnix, (uint)l_CurrentUnixTime);
    }
}
