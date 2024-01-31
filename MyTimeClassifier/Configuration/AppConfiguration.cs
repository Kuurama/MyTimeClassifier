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
        return l_DbContext.Configurations.SingleOrDefault() ?? new Database.Entities.Configuration
        {
            Jobs = DefaultConfiguration.s_Jobs
        };
    }
}
