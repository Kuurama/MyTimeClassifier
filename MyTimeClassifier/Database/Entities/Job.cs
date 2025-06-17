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
    private const uint MaxEmojiLength = 63;
    private const uint MaxTextLength = 48;

    public Job() => OnDeleteCommand = GetDeleteCommand();


    [Key]
    public Guid Id { get; set => SetField(ref field, value); }

    public IBrush StrokeColor { get; set => SetField(ref field, value); }
        = new SolidColorBrush(Color.Parse("#151A23"));

    public IBrush FillColor { get; set => SetField(ref field, value); }
        = new SolidColorBrush(Color.Parse("#191E27"));

    public IBrush ContentColor { get; set => SetField(ref field, value); }
        = new SolidColorBrush(Color.Parse("#FAFAFA"));

    public uint Priority { get; set => SetField(ref field, value); }

    public bool IsRadial
    {
        get;
        set
        {
            SetField(ref field, value);
            AppConfiguration.StaticCache.TriggerReRender();
        }
    }

    [NotMapped]
    public uint PriorityWithReordering
    {
        get => Priority;
        set
        {
            var oldPriority = Priority;
            Priority = value;
            AppConfiguration.StaticCache.ReOrderJobs((JobID: Id, NewValue: value, OldValue: oldPriority));
            AppConfiguration.StaticCache.TriggerReRender();
        }
    }

    public bool Enabled
    {
        get;
        set
        {
            SetField(ref field, value);
            AppConfiguration.StaticCache.TriggerReRender();
        }
    } = true;

    [NotMapped]
    public Color FillColorAsColor
    {
        get => FillColor is SolidColorBrush brush ? brush.Color : Colors.Black;
        set
        {
            if (FillColor is not SolidColorBrush brush)
                return;

            brush.Color = value;
            OnPropertyChanged();
        }
    }

    [NotMapped]
    public Color StrokeColorAsColor
    {
        get => StrokeColor is SolidColorBrush brush ? brush.Color : Colors.White;
        set
        {
            if (StrokeColor is not SolidColorBrush brush)
                return;

            brush.Color = value;
            OnPropertyChanged();
        }
    }

    [NotMapped]
    public Color ContentColorAsColor
    {
        get => ContentColor is SolidColorBrush brush ? brush.Color : Colors.White;
        set
        {
            if (ContentColor is not SolidColorBrush brush)
                return;

            brush.Color = value;
            OnPropertyChanged();
        }
    }

    [MaxLength((int)MaxTextLength)]
    public string Text
    {
        get;
        set
        {
            SetField(ref field, value);
            AppConfiguration.StaticCache.TriggerReRender();
        }
    } = "Unknown";

    [NotMapped]
    public string NormalizedText => Text.Replace("\\n", " ");

    [MaxLength((int)MaxEmojiLength)]
    public string Emoji
    {
        get;
        set
        {
            SetField(ref field, value);
            AppConfiguration.StaticCache.TriggerReRender();
        }
    } = "fa-question";

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