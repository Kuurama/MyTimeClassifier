using MyTimeClassifier.Configuration;
using MyTimeClassifier.Database;
using MyTimeClassifier.Database.Entities;
using System.Linq;

namespace MyTimeClassifier.UI.ViewModels;

public class JobEditWindowViewModel : ViewModelBase
{
    public static void AddNewRow()
    {
        using var l_DbContext     = new AppDbContext();
        var       l_CurrentConfig = AppConfiguration.StaticCache;

        l_DbContext.Attach(l_CurrentConfig);

        var l_TaskPriorities = l_CurrentConfig.Jobs.Select(p_X => p_X.Priority).ToArray();
        var l_FirstAvailablePriority = l_TaskPriorities.Aggregate(1,
            (p_Current, p_NextPriority) => p_NextPriority > p_Current ? p_Current : p_Current + 1);

        l_CurrentConfig.Jobs.Add(new Job((uint)l_FirstAvailablePriority));
        l_DbContext.SaveChanges();

        AppConfiguration.StaticCache.TriggerReRender();
    }
}
