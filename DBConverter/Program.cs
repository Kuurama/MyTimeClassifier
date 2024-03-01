using DBConverter.Context;
using DBConverter.Utils;
using MyTimeClassifier.Configuration;
using MyTimeClassifier.Database;
using MyTimeClassifier.Database.Entities;
using Task = MyTimeClassifier.Database.Entities.Task;

namespace DBConverter;

internal class Program
{
    private static void Main(string[] p_Args)
    {
    Begin:
        var l_OldDbFilePath = ConsoleInputUtils.GetFromConsole<string>("Please provide the path of the old database file:", "Incorrect input, or couldn't locate the file", ConsoleInputUtils.EParseVerificationScheme.FilePath);

        if (!ConsoleInputUtils.BoolFromConsole("Do you want to convert the old database to the new one?"))
        {
            ConsoleInputUtils.PressAnyKeyToExit();
            return;
        }

        Console.WriteLine("Trying to convert the old database to the new one...");

        var l_OldDbContext = new OldDbContext(l_OldDbFilePath);

        if (l_OldDbContext.Database.EnsureCreated())
        {
            Console.WriteLine("The file has been removed between the time it was checked and the time it was used, therefore, it created a new db, please try again with a fresh path");
            if (ConsoleInputUtils.BoolFromConsole("Do you want to try again?"))
                goto Begin;

            return;
        }

        try
        {
            var l_MyTimeClassifierDbContext = new AppDbContext();

            Console.WriteLine("Making sure the new database is completely freshly created...");
            l_MyTimeClassifierDbContext.Database.EnsureDeleted();
            l_MyTimeClassifierDbContext.Database.EnsureCreated();

            Console.WriteLine("Converting the old database to the new one...");
            var l_Configuration = new Configuration(DefaultConfiguration.s_Configuration, false);

            /* Adding the default configuration to the new database */
            l_MyTimeClassifierDbContext.Configurations.Add(l_Configuration);
            l_MyTimeClassifierDbContext.SaveChanges();

            /* We make sure to get the attached configuration from the database now */
            l_Configuration = l_MyTimeClassifierDbContext.Configurations.First();

            /* We make sure to get all the task's job names from the task list, instead of the job list, because the job list might not be complete */
            var l_AllJobName = l_OldDbContext.Tasks.Select(p_X => p_X.JobName)
                .Distinct()
                .ToHashSet();

            /* We make sure to get the currently used jobs that might not have been used yet in the old database */
            foreach (var l_OldJob in l_OldDbContext.JobList)
                l_AllJobName.Add(l_OldJob.nameTache);

            /* Custom check for actual duplicated jobs */
            // Check if l_AllJobName has any duplicates, which mean -> 8 first characters are the same
            var l_DuplicatedGroup = l_AllJobName.Where(p_X => p_X?.Length > 7).GroupBy(p_X => p_X?.Substring(0, 8) ?? string.Empty)
                .Where(p_X => p_X.Count()                                 > 1)
                .Select(p_X => p_X.Key)
                .ToList();

            List<string?> l_AllJobNameFull = [..l_AllJobName];

            /* Remove all but the longest duplicate of each group from the AllJobName list */
            foreach (var l_DuplicateStart in l_DuplicatedGroup)
            {
                var l_SimilarDuplicates = l_AllJobName.Where(p_X => p_X?.StartsWith(l_DuplicateStart) ?? false)
                    .OrderByDescending(p_X => p_X?.Length).ToArray();
                var l_LongestDuplicate = l_SimilarDuplicates.First();

                l_AllJobName.RemoveWhere(p_X => l_SimilarDuplicates.Contains(p_X) && p_X != l_LongestDuplicate);
            }

            var l_Priority = 0u;

            /* Adding the Jobs to the configuration */
            foreach (var l_OldJobName in l_AllJobName)
                l_Configuration.Jobs.Add(new Job(l_OldJobName ?? string.Empty, null, null, null, null, l_Priority++, true));

            /* Adding the Jobs to the DB (so we can then use their IDs to link the tasks to them) */
            l_MyTimeClassifierDbContext.SaveChanges();
            Console.WriteLine("Jobs imported, now importing tasks...");

            /* For performance reasons, we will use a dictionary to link the job names to their IDs */
            var l_NameToJobID = l_MyTimeClassifierDbContext.Jobs.ToDictionary(p_X => p_X.Text, p_X => p_X.Id);

            /* Adding back the duplicated jobs to put to their homologue longer duplicate */
            foreach (var l_DuplicateStart in l_DuplicatedGroup)
            {
                var l_SimilarDuplicates = l_AllJobNameFull.Where(p_X => p_X?.StartsWith(l_DuplicateStart) ?? false)
                    .OrderByDescending(p_X => p_X?.Length).ToArray();
                var l_LongestDuplicate = l_SimilarDuplicates.First();

                if (l_LongestDuplicate is null) continue;

                l_SimilarDuplicates.Where(p_X => p_X != l_LongestDuplicate).ToList().ForEach(p_X =>
                {
                    if (p_X is null) return;

                    l_NameToJobID.Add(p_X, l_NameToJobID[l_LongestDuplicate]);
                });
            }

            uint l_AutoIncrement = 1;

            foreach (var l_OldTask in l_OldDbContext.Tasks)
            {
                if (l_OldTask.JobName == null)
                    continue;

                var l_CurrentJobID = l_NameToJobID[l_OldTask.JobName];
                /* We divide by 1000 to convert the UnixTime from milliseconds to seconds */
                l_MyTimeClassifierDbContext.Tasks.Add(new Task(l_AutoIncrement++, l_CurrentJobID, (uint?)(l_OldTask.dateD / 1000) ?? 0, (uint?)(l_OldTask.dateF / 1000) ?? 0));
            }

            l_MyTimeClassifierDbContext.SaveChanges();

            /* Adding default colors to the jobs that were in the old database but selected */
            var l_OldSelectedJobs = l_OldDbContext.SelectedJobs.ToArray();

            for (var l_Index = 0; l_Index < l_OldSelectedJobs.Length; l_Index++)
            {
                var l_OldSelectedJob = l_OldSelectedJobs[l_Index];
                var l_CurrentJob     = l_Configuration.Jobs.FirstOrDefault(p_X => p_X.Id == l_NameToJobID[l_OldSelectedJob.nameTache ?? string.Empty]);
                if (l_CurrentJob is null) continue;

                l_CurrentJob.FillColor    = DefaultConfiguration.s_Configuration.Jobs[l_Index % DefaultConfiguration.s_Configuration.Jobs.Count].FillColor;
                l_CurrentJob.StrokeColor  = DefaultConfiguration.s_Configuration.Jobs[l_Index % DefaultConfiguration.s_Configuration.Jobs.Count].StrokeColor;
                l_CurrentJob.ContentColor = DefaultConfiguration.s_Configuration.Jobs[l_Index % DefaultConfiguration.s_Configuration.Jobs.Count].ContentColor;
            }

            l_MyTimeClassifierDbContext.SaveChanges();

            Console.WriteLine("Tasks imported successfully!");
            Console.WriteLine($"The new database has been placed in the directory of this application, under the name '{AppDbContext.DATABASE_PATH_NAME}'");
            ConsoleInputUtils.PressAnyKeyToExit();
        }
        catch (Exception l_Exception)
        {
            Console.WriteLine("Something went wrong: {0}", l_Exception);
            ConsoleInputUtils.PressAnyKeyToExit();
        }
    }
}
