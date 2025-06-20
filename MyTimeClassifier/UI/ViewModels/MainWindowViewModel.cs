﻿using System;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using MyTimeClassifier.Configuration;
using MyTimeClassifier.Database;
using MyTimeClassifier.UI.Components;
using MyTimeClassifier.Utils;
using ReactiveUI;

namespace MyTimeClassifier.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel(JobRadialSelector currentJobSelector)
    {
        /* Ensure that the task will be stored when the application is stored or crash */
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.ShutdownRequested += (_, _) => StoreCurrentTaskIfNecessary();

        CurrentClock = new Clock(DefaultConfiguration.AutoSaveIntervalSeconds)
        {
            /* Auto save at interval */
            OnNextCycle = (_, _) =>
            {
                AppConfiguration.SaveConfiguration();
                // Not yet storing on auto-save because it might add a lot of noise to the database.
            }
        };
        CurrentJobSelector = currentJobSelector;
        /* Save the current job and then stop the clock and unselect the job. */
        StopCommand = ReactiveCommand.Create(() =>
        {
            Console.WriteLine("StopCommand invoked");
            Console.WriteLine($"Job deselected was {CurrentJobSelector.SelectedJobID}");

            CurrentClock.Stop();
            StoreCurrentTaskIfNecessary();

            /* Reset the job selector (after it's been saved if necessary) */
            CurrentJobSelector.SelectedJobID = Guid.Empty;
            JobIsSelected = false;
        });
        OnJobSelected = jobID =>
        {
            if (jobID == Guid.Empty) throw new ArgumentException("JobID.None is not a valid job.");

            /* Make sure to stop the current job if it's the same as the one selected. */
            if (CurrentJobSelector.SelectedJobID != Guid.Empty)
                StopCommand.Execute(null);

            Console.WriteLine($"Job selected: {jobID}");
            CurrentJobSelector.SelectedJobID = jobID;
            JobIsSelected = jobID != Guid.Empty;
            SelectedJobText = AppConfiguration.StaticCache.Jobs
                                  .FirstOrDefault(x => x.Id == jobID)?.Text
                              ?? "Unknown";
            CurrentClock.Start();
        };
    }

    [Obsolete("Only for design data context.")]
    public MainWindowViewModel()
    {
        OnJobSelected = _ => { };
        CurrentJobSelector = new JobRadialSelector();
        JobIsSelected = true;
        CurrentClock = new Clock(0);
        StopCommand = ReactiveCommand.Create(() => { });
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public ICommand StopCommand { get; init; }

    public JobRadialSelector CurrentJobSelector { get; init; }
    public Clock CurrentClock { get; init; }

    public bool JobIsSelected { get; set => this.RaiseAndSetIfChanged(ref field, value); }

    public string SelectedJobText
    {
        get => field.Replace("\\n", "\n");
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public Action<Guid> OnJobSelected { get; init; }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Stores the current task if a job is selected, and if the task is longer than 5 seconds.
    /// </summary>
    private void StoreCurrentTaskIfNecessary()
    {
        if (!JobIsSelected) return;

        var startingTimeUnix = CurrentClock.StartingTime.ToUnixTime();
        var currentUnixTime = TimeUtils.UnixTimeNow();

        /* Don't store Tasks that are less than 5 seconds. */
        if (currentUnixTime - startingTimeUnix < 5)
        {
            Console.WriteLine("Shorter than 5 seconds, not storing.");
            return;
        }

        Console.WriteLine($"Storing task from {startingTimeUnix} to {currentUnixTime}");
        /* Store the task in the database. */
        StaticRepo.StoreTask(CurrentJobSelector.SelectedJobID, (uint)startingTimeUnix, (uint)currentUnixTime,
            out _);
    }
}