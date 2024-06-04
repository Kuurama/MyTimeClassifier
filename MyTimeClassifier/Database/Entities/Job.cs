using Avalonia.Media;
using MsBox.Avalonia;
using MyTimeClassifier.Configuration;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reactive;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MyTimeClassifier.Database.Entities;

public class Job : INotifyPropertyChanged
{
    private const uint MAX_EMOJI_LENGTH = 63;
    private const uint MAX_TEXT_LENGTH  = 48;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    private IBrush m_ContentColor = new SolidColorBrush(Color.Parse("#FAFAFA"));
    private string m_Emoji        = "fa-question";
    private bool   m_Enabled      = true;
    private IBrush m_FillColor    = new SolidColorBrush(Color.Parse("#191E27"));
    private Guid   m_Id;
    private bool   m_IsRadial = true;
    private uint   m_Priority;
    private IBrush m_StrokeColor = new SolidColorBrush(Color.Parse("#151A23"));
    private string m_Text        = "Unknown";

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///     Constructor used by Entity Framework.
    /// </summary>
    public Job() => OnDeleteCommand = GetDeleteCommand();

    public Job(uint p_Priority)
    {
        m_Priority      = p_Priority;
        OnDeleteCommand = GetDeleteCommand();
    }

    public Job(string? p_Text, string? p_Emoji, IBrush? p_FillColor, IBrush? p_StrokeColor, IBrush? p_ContentColor, uint p_Priority, bool p_IsRadial, bool p_Enabled = true)
    {
        m_Text         = p_Text         ?? "Unknown";
        m_Emoji        = p_Emoji        ?? "fa-question";
        m_FillColor    = p_FillColor    ?? new SolidColorBrush(Color.Parse("#191E27"));
        m_StrokeColor  = p_StrokeColor  ?? new SolidColorBrush(Color.Parse("#151A23"));
        m_ContentColor = p_ContentColor ?? new SolidColorBrush(Color.Parse("#FAFAFA"));
        m_Priority     = p_Priority;
        m_IsRadial     = p_IsRadial;
        m_Enabled      = p_Enabled;

        OnDeleteCommand = GetDeleteCommand();
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    [Key]
    public Guid Id
    {
        get => m_Id;
        set => SetField(ref m_Id, value);
    }

    public IBrush StrokeColor
    {
        get => m_StrokeColor;
        set => SetField(ref m_StrokeColor, value);
    }

    public IBrush FillColor
    {
        get => m_FillColor;
        set => SetField(ref m_FillColor, value);
    }

    public IBrush ContentColor
    {
        get => m_ContentColor;
        set => SetField(ref m_ContentColor, value);
    }

    public uint SafePriority
    {
        get => m_Priority;
        set => SetField(ref m_Priority, value);
    }

    public uint Priority
    {
        get => m_Priority;
        set
        {
            SetField(ref m_Priority, value);
            AppConfiguration.StaticCache.TriggerReRender();
        }
    }

    public bool IsRadial
    {
        get => m_IsRadial;
        set
        {
            SetField(ref m_IsRadial, value);
            AppConfiguration.StaticCache.TriggerReRender();
        }
    }

    [NotMapped]
    public uint PriorityReordering
    {
        get => m_Priority;
        set
        {
            var l_OldPriority = m_Priority;
            SetField(ref m_Priority, value);
            AppConfiguration.StaticCache.ReOrderJobs((JobID: Id, NewValue: m_Priority, OldValue: l_OldPriority));
            AppConfiguration.StaticCache.TriggerReRender();
        }
    }

    public bool Enabled
    {
        get => m_Enabled;
        set
        {
            SetField(ref m_Enabled, value);
            AppConfiguration.StaticCache.TriggerReRender();
        }
    }

    [NotMapped]
    public Color FillColorAsColor
    {
        get => m_FillColor is SolidColorBrush l_Brush ? l_Brush.Color : Colors.Black;
        set
        {
            if (m_FillColor is not SolidColorBrush l_Brush)
                return;

            l_Brush.Color = value;
            OnPropertyChanged();
        }
    }

    [NotMapped]
    public Color StrokeColorAsColor
    {
        get => m_StrokeColor is SolidColorBrush l_Brush ? l_Brush.Color : Colors.White;
        set
        {
            if (m_StrokeColor is not SolidColorBrush l_Brush)
                return;

            l_Brush.Color = value;
            OnPropertyChanged();
        }
    }

    [NotMapped]
    public Color ContentColorAsColor
    {
        get => m_ContentColor is SolidColorBrush l_Brush ? l_Brush.Color : Colors.White;
        set
        {
            if (m_ContentColor is not SolidColorBrush l_Brush)
                return;

            l_Brush.Color = value;
            OnPropertyChanged();
        }
    }

    [MaxLength((int)MAX_TEXT_LENGTH)]
    public string Text
    {
        get => m_Text;
        set
        {
            SetField(ref m_Text, value);
            AppConfiguration.StaticCache.TriggerReRender();
        }
    }
    
    [NotMapped]
    public string NormalizedText => Text.Replace("\\n", " ");

    [MaxLength((int)MAX_EMOJI_LENGTH)]
    public string Emoji
    {
        get => m_Emoji;
        set
        {
            SetField(ref m_Emoji, value);
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

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    private ReactiveCommand<Unit, Unit> GetDeleteCommand() => ReactiveCommand.Create(() =>
    {
        using var l_DBContext = new AppDbContext();

        if (l_DBContext.Tasks.Any(p_X => p_X.JobID == Id))
        {
            MessageBoxManager.GetMessageBoxStandard("Job has saved tasks", "The job has saved tasks and cannot be deleted.\nIf you wish to delete it, delete or migrate all it's linked tasks first (in the history section)").ShowWindowAsync();
            return;
        }

        l_DBContext.Jobs.Remove(l_DBContext.Jobs.First(p_X => p_X.Id == Id));
        l_DBContext.SaveChanges();

        Id = Guid.Empty;

        AppConfiguration.StaticCache.Jobs.Remove(this);
        AppConfiguration.StaticCache.TriggerReRender();
    });
}
