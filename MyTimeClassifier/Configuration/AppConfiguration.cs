using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MyTimeClassifier.Database;

namespace MyTimeClassifier.Configuration;

public static class AppConfiguration
{
    [field: MaybeNull]
    public static Database.Entities.Configuration StaticCache
        => field ??= LoadConfiguration();

    /// <summary>
    /// Fetches the configuration from the database or creates a new one from the default configuration if none exists.
    /// Uses SingleOrDefault to ensure that only one configuration exists in the database.
    /// </summary>
    /// <returns></returns>
    private static Database.Entities.Configuration LoadConfiguration()
    {
        using var dbContext = new AppDbContext();
        var config = null as Database.Entities.Configuration;

        /* Catch issues with the database (as in program startup) */
        try
        {
            config = dbContext.Configurations.Include(x => x.Jobs.OrderBy(job => job.Priority)).SingleOrDefault();
        }
        catch
        {
            /* Backup the database file */
            if (File.Exists(AppDbContext.DatabasePathName))
                File.Copy(AppDbContext.DatabasePathName,
                    $"{AppDbContext.DatabasePathName}_{DateTimeOffset.Now:yyyy-MM-dd_HH-mm-ss}.bak",
                    true);

            /* Create an alert for the user with avalonia (Doesn't pause the process) */
            MessageBoxManager.GetMessageBoxStandard("Database Error",
                "[MyTimeClassifier] The database file is corrupted. A backup has been created.").ShowWindowAsync();

            /* Ensure deletion of the database file */
            if (dbContext.Database.EnsureDeleted())
            {
                /* Create a new database file */
                dbContext.Database.EnsureCreated();

                MessageBoxManager.GetMessageBoxStandard("Cleaned improper database data",
                    "[MyTimeClassifier] The database has been cleaned-up.").ShowWindowAsync();
            }
        }

        config ??= dbContext.Configurations.Add(DefaultConfiguration.Configuration).Entity;

        // Check if jobs have improper IDs
        if (dbContext.Jobs.Any(x => x.Id == Guid.Empty))
        {
            dbContext.Jobs.RemoveRange(dbContext.Jobs.Where(x => x.Id == Guid.Empty));
            dbContext.SaveChanges();
        }

        // Check if the priority of the jobs is correct
        var priority = 0u;

        foreach (var job in config.Jobs.OrderBy(x => x.Priority))
            if (job.Priority != ++priority)
                job.Priority = priority;

        /* Save the configuration in case a new one was created from the default configuration. */
        dbContext.SaveChanges();
        return config;
    }

    /// <summary>
    /// Saves the configuration singleton instance to the database.
    /// </summary>
    public static void SaveConfiguration()
    {
        using var dbContext = new AppDbContext();
        dbContext.Configurations.Update(StaticCache);
        dbContext.SaveChanges();
    }
}