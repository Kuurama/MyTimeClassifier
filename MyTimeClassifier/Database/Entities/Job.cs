using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reactive;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia.Media;
using MsBox.Avalonia;
using MyTimeClassifier.Configuration;
using ReactiveUI;

namespace MyTimeClassifier.Database.Entities;

public class Job : INotifyPropertyChanged
{
    private const uint MaxEmojiLenght = 63;
    private const uint MaxTextLenght = 48;
    private IBrush _contentColor = new SolidColorBrush(Color.Parse("#FAFAFA"));
    private string _emoji = "fa-question";
    private bool _enabled = true;
    private IBrush _fillColor = new SolidColorBrush(Color.Parse("#191E27"));

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    private Guid _id;
    private bool _isRadial = true;
    private uint _priority;
    private IBrush _strokeColor = new SolidColorBrush(Color.Parse("#151A23"));
    private string _text = "Unknown";

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Constructor used by Entity Framework.
    /// </summary>
    public Job() => OnDeleteCommand = GetDeleteCommand();

    public Job(uint priority)
    {
        _priority = priority;
        OnDeleteCommand = GetDeleteCommand();
    }

    public Job(string? text, string? emoji, IBrush? fillColor, IBrush? strokeColor, IBrush? contentColor,
               uint priority, bool isRadial, bool enabled = true)
    {
        _text = text ?? "Unknown";
        _emoji = emoji ?? "fa-question";
        _fillColor = fillColor ?? new SolidColorBrush(Color.Parse("#191E27"));
        _strokeColor = strokeColor ?? new SolidColorBrush(Color.Parse("#151A23"));
        _contentColor = contentColor ?? new SolidColorBrush(Color.Parse("#FAFAFA"));
        _priority = priority;
        _isRadial = isRadial;
        _enabled = enabled;

        OnDeleteCommand = GetDeleteCommand();
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////


    [Key]
    public Guid Id { get => _id; set => SetField(ref _id, value); }

    public IBrush StrokeColor { get => _strokeColor; set => SetField(ref _strokeColor, value); }

    public IBrush FillColor { get => _fillColor; set => SetField(ref _fillColor, value); }

    public IBrush ContentColor { get => _contentColor; set => SetField(ref _contentColor, value); }

    public uint Priority
    {
        get => _priority;
        set
        {
            SetField(ref _priority, value);
            AppConfiguration.StaticCache.TriggerReRender();
        }
    }

    public bool IsRadial
    {
        get => _isRadial;
        set
        {
            SetField(ref _isRadial, value);
            AppConfiguration.StaticCache.TriggerReRender();
        }
    }

    [NotMapped]
    public uint PriorityWithReordering
    {
        get => _priority;
        set
        {
            var oldPriority = _priority;
            SetField(ref _priority, value);
            AppConfiguration.StaticCache.ReOrderJobs((JobID: Id, NewValue: _priority, OldValue: oldPriority));
            AppConfiguration.StaticCache.TriggerReRender();
        }
    }

    public bool Enabled
    {
        get => _enabled;
        set
        {
            SetField(ref _enabled, value);
            AppConfiguration.StaticCache.TriggerReRender();
        }
    }

    [NotMapped]
    public Color FillColorAsColor
    {
        get => _fillColor is SolidColorBrush brush ? brush.Color : Colors.Black;
        set
        {
            if (_fillColor is not SolidColorBrush brush)
                return;

            brush.Color = value;
            OnPropertyChanged();
        }
    }

    [NotMapped]
    public Color StrokeColorAsColor
    {
        get => _strokeColor is SolidColorBrush brush ? brush.Color : Colors.White;
        set
        {
            if (_strokeColor is not SolidColorBrush brush)
                return;

            brush.Color = value;
            OnPropertyChanged();
        }
    }

    [NotMapped]
    public Color ContentColorAsColor
    {
        get => _contentColor is SolidColorBrush brush ? brush.Color : Colors.White;
        set
        {
            if (_contentColor is not SolidColorBrush brush)
                return;

            brush.Color = value;
            OnPropertyChanged();
        }
    }

    [MaxLength((int)MaxTextLenght)]
    public string Text
    {
        get => _text;
        set
        {
            SetField(ref _text, value);
            AppConfiguration.StaticCache.TriggerReRender();
        }
    }

    [NotMapped]
    public string NormalizedText => Text.Replace("\\n", " ");

    [MaxLength((int)MaxEmojiLenght)]
    public string Emoji
    {
        get => _emoji;
        set
        {
            SetField(ref _emoji, value);
            AppConfiguration.StaticCache.TriggerReRender();
        }
    }

    [NotMapped]
    public ICommand OnDeleteCommand { get; init; }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public event PropertyChangedEventHandler? PropertyChanged;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

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

    private ReactiveCommand<Unit, Unit> GetDeleteCommand() => ReactiveCommand.Create(() =>
    {
        using var dbContext = new AppDbContext();

        if (dbContext.Tasks.Any(x => x.JobID == Id))
        {
            MessageBoxManager.GetMessageBoxStandard(
                    title: "Job has saved tasks",
                    "The job has saved tasks and cannot be deleted.\nIf you wish to delete it, delete or migrate all it's linked tasks first (in the history section)")
                .ShowWindowAsync();
            return;
        }

        dbContext.Jobs.Remove(dbContext.Jobs.First(x => x.Id == Id));
        dbContext.SaveChanges();

        Id = Guid.Empty;

        AppConfiguration.StaticCache.Jobs.Remove(this);
        // Because removing from the collection does not trigger a re-render, we need to do it manually.
        AppConfiguration.StaticCache.TriggerReRender();
    });
}