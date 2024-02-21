using MyTimeClassifier.Database.Entities;
using System.Linq;

namespace MyTimeClassifier.Database;

public static class StaticRepo
{
    public static void StoreTask(Job.JobID p_JobID, uint p_StartingUnixTime, uint p_EndingUnixTime)
    {
        AppDbContext l_DbContext = new();

        /* Get the next (auto-increment) TaskID */
        var l_NextTaskID = l_DbContext.Tasks.OrderBy(p_X => p_X.Id).Select(p_X => p_X.Id).LastOrDefault() + 1;
        l_DbContext.Tasks.Add(new Task
        {
            Id            = l_NextTaskID,
            JobID         = p_JobID,
            UnixStartTime = p_StartingUnixTime,
            UnixEndTime   = p_EndingUnixTime
        });

        l_DbContext.SaveChanges();
    }
}
