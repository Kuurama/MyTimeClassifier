using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Styling;
using MyTimeClassifier.Configuration;
using MyTimeClassifier.UI;

namespace MyTimeClassifier.Database.Entities;

public sealed class Configuration : INotifyPropertyChanged
{
    private const uint MaxProgramNameLength = 63;

    /// <summary>
    /// A property that is sole purpose is to be bound and act as a trigger for re-rendering the UI.
    /// </summary>
    [NotMapped]
    private byte _reRenderProp;

    [Key]
    public uint Id { get; set => SetField(ref field, value); }

    [MaxLength((int)MaxProgramNameLength)]
    public required string ProgramName { get; set => SetField(ref field, value); } = string.Empty;

    public required bool UseLightTheme
    {
        get;
        set
        {
            SetField(ref field, value);

            try
            {
                ((App?)Application.Current)?.ChangeTheme(AppConfiguration.StaticCache.UseLightTheme
                    ? ThemeVariant.Light
                    : ThemeVariant.Dark);
            }
            catch
            {
                // ignored
            }
        }
    }

    public required ObservableCollection<Job> Jobs
    {
        get;
        set
        {
            SetField(ref field, value);
            OnPropertyChanged(nameof(Priorities));
        }
    }

    public required float RadialContentScale { get; set => SetField(ref field, value); }

    public required float GlobalScale { get; set => SetField(ref field, value); }

    public required bool IsMinimalistic { get; set => SetField(ref field, value); }

    public required float InnerRadiusRatio { get; set => SetField(ref field, value); }

    public required uint RadialSelectorRadius { get; set => SetField(ref field, value); }

    public required uint SpacingAngle { get; set => SetField(ref field, value); }

    public required float TimerScale { get; set => SetField(ref field, value); }

    public required float TitleBarScale { get; set => SetField(ref field, value); }

    /// <inheritdoc cref="_reRenderProp" />
    [NotMapped]
    public byte ReRenderProp { get => _reRenderProp; set => SetField(ref _reRenderProp, value); }

    [NotMapped]
    public uint[] Priorities => Jobs.Select(x => x.Priority).ToArray();

    public event PropertyChangedEventHandler? PropertyChanged;

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

    /// <summary>
    /// A function to trigger a re-render of the attaching UI elements.
    /// </summary>
    public void TriggerReRender() => ReRenderProp++;


    public void ReOrderJobs((Guid JobID, uint NewValue, uint OldValue) jobChange)
    {
        Job[]? jobs;

        if (jobChange.NewValue != jobChange.OldValue)
        {
            /* Find the jobs in between the new and old priority */
            jobs = Jobs.Where(x =>
                    x.Priority >= Math.Min(jobChange.OldValue, jobChange.NewValue)
                    && x.Priority <= Math.Max(jobChange.OldValue, jobChange.NewValue)
                    && x.Id != jobChange.JobID)
                .OrderBy(x => x.Priority)
                .ToArray();

            /* Move the jobs in between down */
            foreach (var job in jobs)
                job.Priority = jobChange.NewValue > jobChange.OldValue
                    ? job.Priority - 1
                    : job.Priority + 1;
        }

        /* Reorder the jobs */
        jobs = Jobs.OrderBy(x => x.Priority).ToArray();

        for (var i = 0; i < jobs.Length; i++)
            Jobs.Move(Jobs.IndexOf(jobs[i]), i);
    }
}