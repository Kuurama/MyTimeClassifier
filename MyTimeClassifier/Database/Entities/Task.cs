using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace MyTimeClassifier.Database.Entities;

public class Task : INotifyPropertyChanged
{
    [Key]
    private TaskID m_Id;
    [Key] [ForeignKey(nameof(Job.Id))]
    private Job.JobID m_JobID;

    /* uint32 is fine because it's enough for use until 2106 */
    private uint m_UnixEndTime;
    private uint m_UnixStartTime;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public uint Id
    {
        get => m_Id;
        set => SetField(ref m_Id, value);
    }

    public Job.JobID JobID
    {
        get => m_JobID;
        set => SetField(ref m_JobID, value);
    }

    public uint UnixStartTime
    {
        get => m_UnixStartTime;
        set => SetField(ref m_UnixStartTime, value);
    }

    public uint UnixEndTime
    {
        get => m_UnixEndTime;
        set => SetField(ref m_UnixEndTime, value);
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public event PropertyChangedEventHandler? PropertyChanged;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    private void OnPropertyChanged([CallerMemberName] string? p_PropertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p_PropertyName));

    private bool SetField<T>(ref T p_Field, T p_Value, [CallerMemberName] string? p_PropertyName = null) where T : IEquatable<T>
    {
        if (EqualityComparer<T>.Default.Equals(p_Field, p_Value))
            return false;

        var l_SaveToDB = p_Value switch
        {
            TaskID l_Value    => l_Value != TaskID.None    && !Equals(p_Value, p_Field),
            Job.JobID l_Value => l_Value != Job.JobID.None && !Equals(p_Value, p_Field),
            uint l_Value      => l_Value != 0              && !Equals(p_Value, p_Field),
            _                 => true
        };

        p_Field = p_Value;

        if (!l_SaveToDB)
        {
            OnPropertyChanged(p_PropertyName);
            return true;
        }

        /* Update the object in the database */
        var l_Result = SaveToDB();
        if (l_Result) OnPropertyChanged(p_PropertyName);

        return l_Result;
    }

    private bool SaveToDB()
    {
        /* Update the field in database */
        var l_DBContext = new AppDbContext();
        l_DBContext.Update(this);

        try
        {
            if (l_DBContext.SaveChanges() != 1) return false;
        }
        catch { return false; }

        return true;
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///     A unique identifier for a <see cref="Task" />.
    /// </summary>
    /// <param name="Value"></param>
    public readonly record struct TaskID(uint Value)
    {
        public static readonly TaskID None = new(0);

        /* add implicit conversion from uint to TaskID */
        public static implicit operator TaskID(uint p_Value) => new(p_Value);

        /* add implicit conversion from TaskID to uint */
        public static implicit operator uint(TaskID p_Value) => p_Value.Value;

        public static implicit operator TaskID(int p_Value) => new((uint)p_Value);

        public static implicit operator int(TaskID p_Value) => (int)p_Value.Value;

        public static implicit operator TaskID(long p_Value) => new((uint)p_Value);

        public static implicit operator long(TaskID p_Value) => p_Value.Value;

        public static implicit operator TaskID(ulong p_Value) => new((uint)p_Value);
    }
}
