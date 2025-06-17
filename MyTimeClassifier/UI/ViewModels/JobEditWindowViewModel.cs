using System.Linq;
using MyTimeClassifier.Configuration;
using MyTimeClassifier.Database;
using MyTimeClassifier.Database.Entities;

namespace MyTimeClassifier.UI.ViewModels;

public class JobEditWindowViewModel : ViewModelBase
{
    public static void AddNewRow()
    {
        using var dbContext = new AppDbContext();
        var currentConfig = AppConfiguration.StaticCache;

        dbContext.Attach(currentConfig);

        var taskPriorities = currentConfig.Jobs
            .Select(x => x.Priority)
            .ToArray();

        var firstAvailablePriority = taskPriorities
            .Aggregate(1, (current, nextPriority) => nextPriority > current ? current : current + 1);

        currentConfig.Jobs.Add(new Job { Priority = (uint)firstAvailablePriority });
        dbContext.SaveChanges();

        AppConfiguration.StaticCache.TriggerReRender();
    }
}