using Avalonia.Media;
using Microsoft.EntityFrameworkCore;
using MyTimeClassifier.Database.Entities;

namespace MyTimeClassifier.Database;

public sealed class AppDbContext : DbContext
{
    public const string DATABASE_PATH_NAME = "MyTimeClassifier.db";

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public AppDbContext() => Database.EnsureCreated();

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public DbSet<Entities.Configuration> Configurations { get; set; } = null!;
    public DbSet<Job>                    Jobs           { get; set; } = null!;
    public DbSet<Task>                   Tasks          { get; set; } = null!;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    protected override void OnConfiguring(DbContextOptionsBuilder p_OptionsBuilder)
        => p_OptionsBuilder.UseSqlite($"Data Source={DATABASE_PATH_NAME}");

    /// <summary>
    ///     Specifies the serializer and deserializer for the complex types in the database.
    ///     In this case: JobID, SolidColorBrush.
    /// </summary>
    /// <param name="p_ModelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder p_ModelBuilder)
    {
        p_ModelBuilder.Entity<Job>()
            .Property(p_E => p_E.Id)
            .ValueGeneratedOnAdd();

        p_ModelBuilder.Entity<Task>()
            .Property(p_E => p_E.Id)
            .ValueGeneratedOnAdd();

        p_ModelBuilder.Entity<Job>()
            .Property(p_E => p_E.FillColor)
            .HasConversion(
                p_Brush => p_Brush.ToString() ?? string.Empty,
                p_StoredString => SolidColorBrush.Parse(p_StoredString)
            );

        p_ModelBuilder.Entity<Job>()
            .Property(p_E => p_E.StrokeColor)
            .HasConversion(
                p_Brush => p_Brush.ToString() ?? string.Empty,
                p_StoredString => SolidColorBrush.Parse(p_StoredString)
            );

        p_ModelBuilder.Entity<Job>()
            .Property(p_E => p_E.ContentColor)
            .HasConversion(
                p_Brush => p_Brush.ToString() ?? string.Empty,
                p_StoredString => SolidColorBrush.Parse(p_StoredString)
            );
    }
}
