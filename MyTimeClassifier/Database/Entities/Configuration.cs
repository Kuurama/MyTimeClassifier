using Avalonia;
using Avalonia.Styling;
using MyTimeClassifier.Configuration;
using MyTimeClassifier.UI;
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
    private ObservableCollection<Job> m_Jobs = new();

    [MaxLength((int)MAX_PROGRAM_NAME_LENGTH)]
    private string m_ProgramName = string.Empty;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    private float m_RadialContentScale;
    private uint  m_RadialSelectorRadius;
    private uint  m_SpacingAngle;
    private float m_TimerScale;
    private float m_TitleBarScale;

    private bool m_UseLightTheme;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public Configuration() { }

    public Configuration(uint                      p_Id,
                         string                    p_ProgramName,
                         uint                      p_RadialSelectorRadius,
                         float                     p_InnerRadiusRatio,
                         bool                      p_UseLightTheme,
                         bool                      p_IsMinimalistic,
                         uint                      p_SpacingAngle,
                         float                     p_RadialContentScale,
                         float                     p_GlobalScale,
                         float                     p_TimerScale,
                         float                     p_TitleBarScale,
                         ObservableCollection<Job> p_Jobs)
    {
        m_Id                   = p_Id;
        m_ProgramName          = p_ProgramName;
        m_RadialSelectorRadius = p_RadialSelectorRadius;
        m_InnerRadiusRatio     = p_InnerRadiusRatio;
        m_UseLightTheme        = p_UseLightTheme;
        m_IsMinimalistic       = p_IsMinimalistic;
        m_SpacingAngle         = p_SpacingAngle;
        m_RadialContentScale   = p_RadialContentScale;
        m_GlobalScale          = p_GlobalScale;
        m_TimerScale           = p_TimerScale;
        m_TitleBarScale        = p_TitleBarScale;
        m_Jobs                 = p_Jobs;
    }

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
        set
        {
            SetField(ref m_UseLightTheme, value);

            try
            {
                ((App?)Application.Current)?.ChangeTheme(AppConfiguration.StaticCache.UseLightTheme ? ThemeVariant.Light : ThemeVariant.Dark);
            }
            catch
            {
                // ignored
            }
        }
    }

    public ObservableCollection<Job> Jobs
    {
        get => m_Jobs;
        set => SetField(ref m_Jobs, value);
    }

    public float RadialContentScale
    {
        get => m_RadialContentScale;
        set => SetField(ref m_RadialContentScale, value);
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

    public float TimerScale
    {
        get => m_TimerScale;
        set => SetField(ref m_TimerScale, value);
    }

    public float TitleBarScale
    {
        get => m_TitleBarScale;
        set => SetField(ref m_TitleBarScale, value);
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
