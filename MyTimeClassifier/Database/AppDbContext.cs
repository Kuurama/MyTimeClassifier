using Avalonia.Media;
using Microsoft.EntityFrameworkCore;
using MyTimeClassifier.Database.Entities;

namespace MyTimeClassifier.Database;

public sealed class AppDbContext : DbContext
{
    public const string DatabasePathName = "MyTimeClassifier.db";

    public DbSet<Entities.Configuration> Configurations { get; set; } = null!;
    public DbSet<Job> Jobs { get; set; } = null!;
    public DbSet<Task> Tasks { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={DatabasePathName}");

    /// <summary>
    /// Specifies the mapping of the database entities, including the serializer and deserializer for the complex types
    /// in the database. In this case: SolidColorBrush.
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Job>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Task>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Job>()
            .Property(x => x.FillColor)
            .HasConversion(
                brush => brush.ToString() ?? string.Empty,
                storedString => SolidColorBrush.Parse(storedString)
            );

        modelBuilder.Entity<Job>()
            .Property(x => x.StrokeColor)
            .HasConversion(
                brush => brush.ToString() ?? string.Empty,
                x => SolidColorBrush.Parse(x)
            );

        modelBuilder.Entity<Job>()
            .Property(x => x.ContentColor)
            .HasConversion(
                brush => brush.ToString() ?? string.Empty,
                storedString => SolidColorBrush.Parse(storedString)
            );
    }
}