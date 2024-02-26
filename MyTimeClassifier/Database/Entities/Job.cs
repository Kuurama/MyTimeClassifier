using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace MyTimeClassifier.Database.Entities;

public class Job : INotifyPropertyChanged
{
    private const uint MAX_EMOJI_LENGTH = 63;
    private const uint MAX_TEXT_LENGTH  = 12;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    private IBrush? m_ContentColor;
    private string? m_Emoji;
    private IBrush? m_FillColor;
    private Guid    m_Id;
    private IBrush? m_StrokeColor;
    private string? m_Text;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///     Constructor used by Entity Framework.
    /// </summary>
    public Job() { }

    public Job(string? p_Text, string? p_Emoji, IBrush? p_FillColor, IBrush? p_StrokeColor, IBrush? p_ContentColor)
    {
        m_Text         = p_Text;
        m_Emoji        = p_Emoji;
        m_FillColor    = p_FillColor;
        m_StrokeColor  = p_StrokeColor;
        m_ContentColor = p_ContentColor;
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    [Key]
    public Guid Id
    {
        get => m_Id;
        set => SetField(ref m_Id, value);
    }

    public IBrush? StrokeColor
    {
        get => m_StrokeColor;
        set => SetField(ref m_StrokeColor, value);
    }

    public IBrush? FillColor
    {
        get => m_FillColor;
        set => SetField(ref m_FillColor, value);
    }

    public IBrush? ContentColor
    {
        get => m_ContentColor;
        set => SetField(ref m_ContentColor, value);
    }

    [MaxLength((int)MAX_TEXT_LENGTH)]
    public string? Text
    {
        get => m_Text;
        set => SetField(ref m_Text, value);
    }

    [MaxLength((int)MAX_EMOJI_LENGTH)]
    public string? Emoji
    {
        get => m_Emoji;
        set => SetField(ref m_Emoji, value);
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
