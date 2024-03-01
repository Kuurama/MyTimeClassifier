using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MyTimeClassifier.Database;
using System;
using System.IO;
using System.Linq;

namespace MyTimeClassifier.Configuration;

public sealed class AppConfiguration
{
    public static readonly Database.Entities.Configuration StaticCache = LoadConfiguration();

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///     Fetches the configuration from the database or creates a new one from the default configuration if none exists.
    ///     Uses SingleOrDefault to ensure that only one configuration exists in the database.
    /// </summary>
    /// <returns></returns>
    private static Database.Entities.Configuration LoadConfiguration()
    {
        using var l_DbContext = new AppDbContext();
        var       l_Config    = null as Database.Entities.Configuration;

        /* Catch issues with the database (as in program startup) */
        try { l_Config = l_DbContext.Configurations.Include(p_X => p_X.Jobs.OrderBy(p_Job => p_Job.Priority)).SingleOrDefault(); }
        catch
        {
            /* Backup the database file */
            if (File.Exists(AppDbContext.DATABASE_PATH_NAME))
            {
                File.Copy(AppDbContext.DATABASE_PATH_NAME, $"{AppDbContext.DATABASE_PATH_NAME}.bak", true);
            }

            /* Create an alert for the user with avalonia (Doesn't pause the process) */
            MessageBoxManager.GetMessageBoxStandard("Database Error", "[MyTimeClassifier] The database file is corrupted. A backup has been created.").ShowWindowAsync();

            /* Ensure deletion of the database file */
            if (l_DbContext.Database.EnsureDeleted())
            {
                /* Create a new database file */
                l_DbContext.Database.EnsureCreated();

                MessageBoxManager.GetMessageBoxStandard("Cleaned improper database data", "[MyTimeClassifier] The database has been cleaned-up.").ShowWindowAsync();
            }
        }

        l_Config ??= l_DbContext.Configurations.Add(DefaultConfiguration.s_Configuration).Entity;

        // Check if jobs have improper IDs
        if (l_DbContext.Jobs.Any(p_X => p_X.Id == Guid.Empty))
        {
            l_DbContext.Jobs.RemoveRange(l_DbContext.Jobs.Where(p_X => p_X.Id == Guid.Empty));
            l_DbContext.SaveChanges();
        }

        // Check if the priority of the jobs is correct
        var l_Priority = 0u;

        foreach (var l_Job in l_Config.Jobs.OrderBy(p_X => p_X.Priority))
        {
            if (l_Job.SafePriority != ++l_Priority)
            {
                l_Job.SafePriority = l_Priority;
            }
        }

        /* Save the configuration in case a new one was created from the default configuration. */
        l_DbContext.SaveChanges();
        return l_Config;
    }

    /// <summary>
    ///     Saves the configuration singleton instance to the database.
    /// </summary>
    public static void SaveConfiguration()
    {
        using var l_DbContext = new AppDbContext();
        l_DbContext.Configurations.Update(StaticCache);
        l_DbContext.SaveChanges();
    }
}
