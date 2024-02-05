using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace MyTimeClassifier.Database.Entities;

public sealed class Configuration : INotifyPropertyChanged
{
    private const uint MAX_PROGRAM_NAME_LENGTH = 63;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    [Key]
    private uint m_Id;
    private ObservableCollection<Job> m_Jobs = [];

    [MaxLength((int)MAX_PROGRAM_NAME_LENGTH)]
    private string m_ProgramName = "MyTimeClassifier";

    private bool m_UseLightTheme;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public uint Id
    {
        get => m_Id;
        set => SetField(ref m_Id, value);
    }

    [MaxLength((int)MAX_PROGRAM_NAME_LENGTH)]
    public string ProgramName
    {
        get => m_ProgramName;
        set => SetField(ref m_ProgramName, value);
    }

    public bool UseLightTheme
    {
        get => m_UseLightTheme;
        set => SetField(ref m_UseLightTheme, value);
    }

    public ObservableCollection<Job> Jobs
    {
        get => m_Jobs;
        set => SetField(ref m_Jobs, value);
    }

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
}
