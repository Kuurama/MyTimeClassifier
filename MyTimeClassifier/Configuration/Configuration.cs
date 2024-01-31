using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyTimeClassifier.Configuration;

public sealed class Configuration : INotifyPropertyChanged
{
    public static readonly Configuration Instance = new();

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////


    private string m_ProgramName = "MyTimeClassifier";

    public string ProgramName
    {
        get => m_ProgramName;
        set => SetField(ref m_ProgramName, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////


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
