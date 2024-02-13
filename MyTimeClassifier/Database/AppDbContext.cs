using Avalonia.Media;
using Microsoft.EntityFrameworkCore;
using MyTimeClassifier.Database.Entities;

namespace MyTimeClassifier.Database;

public sealed class AppDbContext : DbContext
{
    public AppDbContext() => Database.EnsureCreated();

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public DbSet<Entities.Configuration> Configurations { get; set; } = null!;
    public DbSet<Job>                    Jobs           { get; set; } = null!;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    protected override void OnConfiguring(DbContextOptionsBuilder p_OptionsBuilder)
        => p_OptionsBuilder.UseSqlite("Data Source=MyTimeClassifier.db");

    /// <summary>
    ///     Ensure that the <see cref="IBrush" /> properties of <see cref="Job" /> are stored as strings
    ///     in the database (As complex types are not supported by default).
    /// </summary>
    /// <param name="p_ModelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder p_ModelBuilder)
    {
        p_ModelBuilder.Entity<Job>()
            .Property(p_E => p_E.Id)
            .HasConversion(
                p_JobID => p_JobID.Value,
                p_Id => new Job.JobID(p_Id))
            .ValueGeneratedOnAdd();

        p_ModelBuilder.Entity<Job>()
            .Property(p_E => p_E.FillColor)
            .HasConversion(
                p_Brush => p_Brush != null
                    ? p_Brush.ToString()
                    : null,
                p_StoredString => p_StoredString != null
                    ? SolidColorBrush.Parse(p_StoredString)
                    : null);

        p_ModelBuilder.Entity<Job>()
            .Property(p_E => p_E.StrokeColor)
            .HasConversion(
                p_Brush => p_Brush != null
                    ? p_Brush.ToString()
                    : null,
                p_StoredString => p_StoredString != null
                    ? SolidColorBrush.Parse(p_StoredString)
                    : null);

        p_ModelBuilder.Entity<Job>()
            .Property(p_E => p_E.ContentColor)
            .HasConversion(
                p_Brush => p_Brush != null
                    ? p_Brush.ToString()
                    : null,
                p_StoredString => p_StoredString != null
                    ? SolidColorBrush.Parse(p_StoredString)
                    : null);
    }
}
