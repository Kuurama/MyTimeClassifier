using MyTimeClassifier.Database.Entities;
using MyTimeClassifier.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyTimeClassifier.Database;

public static class StaticRepo
{
    public static void StoreTask(Guid p_JobID, uint p_StartingUnixTime, uint p_EndingUnixTime, out uint p_NewTaskID)
    {
        AppDbContext l_DbContext = new();

        /* Get the next (auto-increment) TaskID */
        p_NewTaskID = l_DbContext.Tasks.OrderBy(p_X => p_X.Id).Select(p_X => p_X.Id).LastOrDefault() + 1;
        l_DbContext.Tasks.Add(new Task(p_NewTaskID, p_JobID, p_StartingUnixTime, p_EndingUnixTime));

        l_DbContext.SaveChanges();
    }

    public static List<Task> GetTasks(int p_Page, int p_ItemsPerPage, int p_AddSkip = 0, int p_AddTake = 0)
    {
        using var l_DbContext = new AppDbContext();
        return l_DbContext.Tasks.OrderBy(p_X => p_X.UnixStartTime)
            .Skip(p_Page * p_ItemsPerPage + p_AddSkip)
            .Take(p_ItemsPerPage          + p_AddTake)
            .ToList();
    }

    /// <summary>
    ///     Returns a list of TaskModels containing their respective Job
    /// </summary>
    /// <param name="p_Page"></param>
    /// <param name="p_ItemsPerPage"></param>
    /// <param name="p_AddSkip"></param>
    /// <param name="p_AddTake"></param>
    /// <returns></returns>
    public static IEnumerable<TaskModel> GetTaskModels(int p_Page, int p_ItemsPerPage, int p_AddSkip = 0, int p_AddTake = 0)
    {
        using var l_DbContext = new AppDbContext();
        return l_DbContext.Tasks.OrderByDescending(p_X => p_X.UnixStartTime)
            .Skip(p_Page * p_ItemsPerPage + p_AddSkip)
            .Take(p_ItemsPerPage          + p_AddTake)
            .Select(p_X => new TaskModel(p_X)).ToArray();
    }

    public static int GetTotalTaskCount()
    {
        using var l_DbContext = new AppDbContext();
        return l_DbContext.Tasks.Count();
    }
}
