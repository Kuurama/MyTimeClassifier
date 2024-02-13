using MyTimeClassifier.Database;
using System.Linq;

namespace MyTimeClassifier.Configuration;

public sealed class AppConfiguration
{
    public static readonly Database.Entities.Configuration StaticCache = LoadConfiguration();

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    private static Database.Entities.Configuration LoadConfiguration()
    {
        using var l_DbContext = new AppDbContext();
        /// Using SingleOrDefault() instead of FirstOrDefault() to ensure that there is only one configuration.
        /// At least, to the current state of the program.
        /// Since this executes the constructor of the Configuration class, it is important to ensure that the default configuration is in it's constructor too.
        var l_Config = l_DbContext.Configurations.SingleOrDefault();
        l_Config ??= l_DbContext.Configurations.Add(DefaultConfiguration.s_Configuration).Entity;
        l_DbContext.SaveChanges();

        return l_Config;
    }

    public static void SaveConfiguration()
    {
        using var l_DbContext = new AppDbContext();
        l_DbContext.Update(StaticCache);
        l_DbContext.SaveChanges();
    }
}
