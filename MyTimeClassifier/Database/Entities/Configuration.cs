using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace MyTimeClassifier.Database.Entities;

public sealed class Configuration : INotifyPropertyChanged
{
    private const uint  MAX_PROGRAM_NAME_LENGTH = 63;
    private       float m_GlobalScale;

    [Key]
    private uint m_Id;
    private float                     m_InnerRadiusRatio;
    private bool                      m_IsMinimalistic;
    private ObservableCollection<Job> m_Jobs = [];

    [MaxLength((int)MAX_PROGRAM_NAME_LENGTH)]
    private string m_ProgramName = "MyTimeClassifier";

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    private float m_RadialRadialContentScale;
    private uint  m_RadialSelectorRadius;
    private uint  m_SpacingAngle;

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

    public float RadialContentScale
    {
        get => m_RadialRadialContentScale;
        set => SetField(ref m_RadialRadialContentScale, value);
    }

    public float GlobalScale
    {
        get => m_GlobalScale;
        set => SetField(ref m_GlobalScale, value);
    }

    public bool IsMinimalistic
    {
        get => m_IsMinimalistic;
        set => SetField(ref m_IsMinimalistic, value);
    }

    public float InnerRadiusRatio
    {
        get => m_InnerRadiusRatio;
        set => SetField(ref m_InnerRadiusRatio, value);
    }

    public uint RadialSelectorRadius
    {
        get => m_RadialSelectorRadius;
        set => SetField(ref m_RadialSelectorRadius, value);
    }

    public uint SpacingAngle
    {
        get => m_SpacingAngle;
        set => SetField(ref m_SpacingAngle, value);
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
