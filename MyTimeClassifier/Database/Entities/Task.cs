using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MyTimeClassifier.Database.Entities;

[PrimaryKey(nameof(Id), nameof(JobID))]
public class Task : INotifyPropertyChanged
{
    private uint m_Id;
    private Guid m_JobID;

    /* uint32 is fine because it's enough for use until 2106 */
    private uint m_UnixEndTime;
    private uint m_UnixStartTime;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///     Constructor used by Entity Framework.
    /// </summary>
    public Task() { }

    public Task(uint p_Id, Guid p_JobID, uint p_UnixStartTime, uint p_UnixEndTime)
    {
        m_Id            = p_Id;
        m_JobID         = p_JobID;
        m_UnixStartTime = p_UnixStartTime;
        m_UnixEndTime   = p_UnixEndTime;
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    [Key]
    public uint Id
    {
        get => m_Id;
        set => SetField(ref m_Id, value);
    }

    [ForeignKey(nameof(Job.Id))]
    public Guid JobID
    {
        get => m_JobID;
        set => SetField(ref m_JobID, value);
    }

    public uint UnixStartTime
    {
        get => m_UnixStartTime;
        set
        {
            /* Throw if the value is higher than the end time */
            if (value > UnixEndTime)
                throw new ArgumentOutOfRangeException(nameof(UnixStartTime),
                    "The start time cannot be higher than the end time.");

            SetField(ref m_UnixStartTime, value);
        }
    }

    public uint UnixEndTime
    {
        get => m_UnixEndTime;
        set
        {
            /* Throw if the value is lower than the start time */
            if (value < UnixStartTime)
                throw new ArgumentOutOfRangeException(nameof(UnixEndTime),
                    "The end time cannot be lower than the start time.");

            SetField(ref m_UnixEndTime, value);
        }
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public event PropertyChangedEventHandler? PropertyChanged;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    private void OnPropertyChanged([CallerMemberName] string? p_PropertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p_PropertyName));

    private static bool SetRefSilent<T>([DisallowNull] ref T p_Field, T p_Value)
    {
        if (p_Field == null) throw new ArgumentNullException(nameof(p_Field));

        p_Field = p_Value;
        return true;
    }

    private bool SetField<T>(ref T p_Field, T p_Value, [CallerMemberName] string? p_PropertyName = null)
        where T : IEquatable<T>
    {
        if (EqualityComparer<T>.Default.Equals(p_Field, p_Value))
            return false;

        return p_Value switch
        {
            Guid l_Value => l_Value != Guid.Empty && UpdateJobID(l_Value),
            uint when p_PropertyName is not nameof(Id) => SetRefSilent(ref p_Field, p_Value) && SaveToDB(),
            (uint)0 when p_PropertyName is nameof(Id) => DeleteFromDB() && SetRefSilent(ref p_Field, p_Value),
            _ => false
        };
    }

    private bool UpdateJobID(Guid p_NewJobID)
    {
        /* We can't update a primary key so it needs creating a new one, deleting current and editing this instance */
        StaticRepo.StoreTask(p_NewJobID, m_UnixStartTime, m_UnixEndTime, out var l_NewTaskID);
        var l_DeleteResult = DeleteFromDB();
        if (!l_DeleteResult) return l_DeleteResult;

        m_JobID = p_NewJobID;
        m_Id = l_NewTaskID;

        OnPropertyChanged(nameof(JobID));
        return l_DeleteResult;
    }

    private bool SaveToDB()
    {
        /* Update the field in database */
        using var l_DBContext = new AppDbContext();
        l_DBContext.Tasks.Update(this);

        try
        {
            if (l_DBContext.SaveChanges() != 1) return false;
        }
        catch
        {
            return false;
        }

        return true;
    }

    private bool DeleteFromDB()
    {
        /* Delete the field from database */
        using var l_DBContext = new AppDbContext();
        l_DBContext.Tasks.Remove(this);

        try
        {
            if (l_DBContext.SaveChanges() != 1) return false;
        }
        catch
        {
            return false;
        }

        return true;
    }
}