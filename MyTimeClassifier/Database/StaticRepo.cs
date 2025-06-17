using System;
using System.Collections.Generic;
using System.Linq;
using MyTimeClassifier.Database.Entities;
using MyTimeClassifier.UI.Models;

namespace MyTimeClassifier.Database;

public static class StaticRepo
{
    public static void StoreTask(Guid jobId, uint startingUnixTime, uint endingUnixTime, out uint newTaskID)
    {
        using var dbContext = new AppDbContext();

        /* Get the next (auto-increment) TaskID */
        newTaskID = dbContext.Tasks.OrderBy(x => x.Id).Select(x => x.Id).LastOrDefault() + 1;
        dbContext.Tasks.Add(new Task
        {
            Id = newTaskID,
            JobID = jobId,
            UnixStartTime = startingUnixTime,
            UnixEndTime = endingUnixTime
        });

        dbContext.SaveChanges();
    }

    public static List<Task> GetTasks(int page, int itemsPerPage, int addSkip = 0, int addTake = 0)
    {
        using var dbContext = new AppDbContext();
        return dbContext.Tasks.OrderBy(x => x.UnixStartTime)
            .Skip(page * itemsPerPage + addSkip)
            .Take(itemsPerPage + addTake)
            .ToList();
    }

    /// <summary>
    /// Returns a list of TaskModels containing their respective Job
    /// </summary>
    /// <param name="page"></param>
    /// <param name="itemsPerPage"></param>
    /// <param name="addSkip"></param>
    /// <param name="addTake"></param>
    /// <returns></returns>
    public static IEnumerable<TaskModel> GetTaskModels(int page, int itemsPerPage, int addSkip = 0, int addTake = 0)
    {
        using var dbContext = new AppDbContext();
        return dbContext.Tasks.OrderByDescending(x => x.UnixStartTime)
            .Skip(page * itemsPerPage + addSkip)
            .Take(itemsPerPage + addTake)
            .Select(x => new TaskModel(x)).ToArray();
    }

    public static int GetTotalTaskCount()
    {
        using var dbContext = new AppDbContext();
        return dbContext.Tasks.Count();
    }
}