using MyTimeClassifier.Configuration;
using MyTimeClassifier.Database;
using MyTimeClassifier.Database.Entities;

namespace MyTimeClassifier.UI.ViewModels;

public class JobEditWindowViewModel : ViewModelBase
{
    public static void AddNewRow()
    {
        var l_DbContext     = new AppDbContext();
        var l_CurrentConfig = AppConfiguration.StaticCache;

        l_DbContext.Attach(l_CurrentConfig);
        l_CurrentConfig.Jobs.Add(new Job());
        l_DbContext.SaveChanges();

        AppConfiguration.StaticCache.TriggerReRender();
    }
}
