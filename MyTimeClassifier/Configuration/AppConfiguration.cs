using Microsoft.EntityFrameworkCore;
using MyTimeClassifier.Database;
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

        var l_Config = l_DbContext.Configurations.Include(p_X => p_X.Jobs).SingleOrDefault();
        l_Config ??= l_DbContext.Configurations.Add(DefaultConfiguration.s_Configuration).Entity;
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
