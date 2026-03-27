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

    private float _globalScale;

    private uint _id;
    private float _innerRadiusRatio;
    private bool _isMinimalistic;
    private ObservableCollection<Job> _jobs = new();


    private string _programName = string.Empty;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    private float _radialContentScale;
    private uint _radialSelectorRadius;

    /// <summary>
    /// A property that is sole purpose is to be bound and act as a trigger for re-rendering the UI.
    /// </summary>
    [NotMapped]
    private byte _reRenderProp;

    private uint _spacingAngle;
    private float _timerScale;
    private float _titleBarScale;

    private bool _useLightTheme;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Constructor used by Entity Framework.
    /// </summary>
    public Configuration() { }

    public Configuration(
        uint id, string programName, uint radialSelectorRadius, float innerRadiusRatio, bool useLightTheme,
        bool isMinimalistic, uint spacingAngle, float radialContentScale, float globalScale, float timerScale,
        float titleBarScale, ObservableCollection<Job> jobs)
    {
        _id = id;
        _programName = programName;
        _radialSelectorRadius = radialSelectorRadius;
        _innerRadiusRatio = innerRadiusRatio;
        _useLightTheme = useLightTheme;
        _isMinimalistic = isMinimalistic;
        _spacingAngle = spacingAngle;
        _radialContentScale = radialContentScale;
        _globalScale = globalScale;
        _timerScale = timerScale;
        _titleBarScale = titleBarScale;
        _jobs = jobs;
    }

    public Configuration(Configuration configuration, bool referenceJobs)
    {
        _id = configuration.Id;
        _programName = configuration.ProgramName;
        _radialSelectorRadius = configuration.RadialSelectorRadius;
        _innerRadiusRatio = configuration.InnerRadiusRatio;
        _useLightTheme = configuration.UseLightTheme;
        _isMinimalistic = configuration.IsMinimalistic;
        _spacingAngle = configuration.SpacingAngle;
        _radialContentScale = configuration.RadialContentScale;
        _globalScale = configuration.GlobalScale;
        _timerScale = configuration.TimerScale;
        _titleBarScale = configuration.TitleBarScale;
        if (referenceJobs)
            _jobs = configuration.Jobs;
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    [Key]
    public uint Id { get => _id; set => SetField(ref _id, value); }

    [MaxLength((int)MaxProgramNameLength)]
    public string ProgramName { get => _programName; set => SetField(ref _programName, value); }

    public bool UseLightTheme
    {
        get => _useLightTheme;
        set
        {
            SetField(ref _useLightTheme, value);

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

    public ObservableCollection<Job> Jobs
    {
        get => _jobs;
        set
        {
            SetField(ref _jobs, value);
            OnPropertyChanged(nameof(Priorities));
        }
    }

    public float RadialContentScale { get => _radialContentScale; set => SetField(ref _radialContentScale, value); }

    public float GlobalScale { get => _globalScale; set => SetField(ref _globalScale, value); }

    public bool IsMinimalistic { get => _isMinimalistic; set => SetField(ref _isMinimalistic, value); }

    public float InnerRadiusRatio { get => _innerRadiusRatio; set => SetField(ref _innerRadiusRatio, value); }

    public uint RadialSelectorRadius
    {
        get => _radialSelectorRadius;
        set => SetField(ref _radialSelectorRadius, value);
    }

    public uint SpacingAngle { get => _spacingAngle; set => SetField(ref _spacingAngle, value); }

    public float TimerScale { get => _timerScale; set => SetField(ref _timerScale, value); }

    public float TitleBarScale { get => _titleBarScale; set => SetField(ref _titleBarScale, value); }

    /// <inheritdoc cref="_reRenderProp" />
    [NotMapped]
    public byte ReRenderProp { get => _reRenderProp; set => SetField(ref _reRenderProp, value); }

    [NotMapped]
    public uint[] Priorities => _jobs.Select(x => x.Priority).ToArray();

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

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
        Job[]? newJobsOrdering;

        if (jobChange.NewValue != jobChange.OldValue)
        {
            /* Find the jobs in between the new and old priority */
            newJobsOrdering = Jobs.Where(x =>
                    x.Priority >= Math.Min(jobChange.OldValue, jobChange.NewValue)
                    && x.Priority <= Math.Max(jobChange.OldValue, jobChange.NewValue)
                    && x.Id != jobChange.JobID)
                .OrderBy(x => x.Priority)
                .ToArray();

            /* Move the jobs in between down */
            foreach (var job in newJobsOrdering)
                job.Priority = jobChange.NewValue > jobChange.OldValue
                    ? job.Priority - 1
                    : job.Priority + 1;
        }

        /* Reorder the jobs */
        newJobsOrdering = Jobs.OrderBy(x => x.Priority).ToArray();

        for (var i = 0; i < newJobsOrdering.Length; i++)
            _jobs.Move(_jobs.IndexOf(newJobsOrdering[i]), i);
    }
}