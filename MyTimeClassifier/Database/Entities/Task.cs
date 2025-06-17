using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace MyTimeClassifier.Database.Entities;

[PrimaryKey(nameof(Id), nameof(JobID))]
public class Task : INotifyPropertyChanged
{
    /// <summary>
    /// Constructor used by Entity Framework.
    /// </summary>
    public Task() { }

    [Key]
    public uint Id { get; set => SetField(ref field, value); }

    [ForeignKey(nameof(Job.Id))]
    public Guid JobID { get; set => SetField(ref field, value); }

    public uint UnixStartTime
    {
        get;
        set
        {
            /* Throw if the value is higher than the end time */
            if (UnixEndTime != 0 && value > UnixEndTime)
                throw new ArgumentOutOfRangeException(nameof(UnixStartTime),
                    "The start time cannot be higher than the end time.");

            SetField(ref field, value);
        }
    }

    /* uint32 is fine because it's enough for use until 2106 */
    public uint UnixEndTime
    {
        get;
        set
        {
            /* Throw if the value is lower than the start time */
            if (UnixStartTime != 0 && value < UnixStartTime)
                throw new ArgumentOutOfRangeException(nameof(UnixEndTime),
                    "The end time cannot be lower than the start time.");

            SetField(ref field, value);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private static bool SetRefSilent<T>([DisallowNull] ref T field, T value)
    {
        if (field == null) throw new ArgumentNullException(nameof(field));

        field = value;
        return true;
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        where T : IEquatable<T>
    {
        if (field.Equals(default))
        {
            field = value;
            return true;
        }

        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        return value switch
        {
            Guid jobId => jobId != Guid.Empty && UpdateJobId(jobId),
            uint when propertyName is not nameof(Id) => SetRefSilent(ref field, value) && SaveToDb(),
            (uint)0 when propertyName is nameof(Id) => DeleteFromDb() && SetRefSilent(ref field, value),
            _ => false
        };
    }

    private bool UpdateJobId(Guid newJobID)
    {
        // We can't update a primary key so it needs creating a new one, deleting current and editing this instance
        StaticRepo.StoreTask(newJobID, UnixStartTime, UnixEndTime, out var newTaskID);
        if (!DeleteFromDb())
            return false;

        Id = newTaskID;

        OnPropertyChanged(nameof(JobID));
        return true;
    }

    private bool SaveToDb()
    {
        using var dbContext = new AppDbContext();
        dbContext.Tasks.Update(this);

        try
        {
            if (dbContext.SaveChanges() != 1) return false;
        }
        catch
        {
            return false;
        }

        return true;
    }

    private bool DeleteFromDb()
    {
        using var dbContext = new AppDbContext();
        dbContext.Tasks.Remove(this);

        try
        {
            if (dbContext.SaveChanges() != 1) return false;
        }
        catch
        {
            return false;
        }

        return true;
    }
}