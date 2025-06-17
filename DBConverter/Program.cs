using DBConverter.Context;
using DBConverter.Utils;
using MyTimeClassifier.Configuration;
using MyTimeClassifier.Database;
using MyTimeClassifier.Database.Entities;
using Task = MyTimeClassifier.Database.Entities.Task;

namespace DBConverter;

internal class Program
{
    private static void Main(string[] args)
    {
    Begin:
        var oldDbFilePath = ConsoleInputUtils.GetFromConsole<string>(
            "Please provide the path of the old database file:", "Incorrect input, or couldn't locate the file",
            ConsoleInputUtils.EParseVerificationScheme.FilePath);

        if (!ConsoleInputUtils.BoolFromConsole("Do you want to convert the old database to the new one?"))
        {
            ConsoleInputUtils.PressAnyKeyToExit();
            return;
        }

        Console.WriteLine("Trying to convert the old database to the new one...");

        var odlDbContext = new OldDbContext(oldDbFilePath);

        if (odlDbContext.Database.EnsureCreated())
        {
            Console.WriteLine(
                "The file has been removed between the time it was checked and the time it was used, therefore, it created a new db, please try again with a fresh path");
            if (ConsoleInputUtils.BoolFromConsole("Do you want to try again?"))
                goto Begin;

            return;
        }

        try
        {
            var myTimeClassifierDbContext = new AppDbContext();

            Console.WriteLine("Making sure the new database is completely freshly created...");
            myTimeClassifierDbContext.Database.EnsureDeleted();
            myTimeClassifierDbContext.Database.EnsureCreated();

            Console.WriteLine("Converting the old database to the new one...");
            var configuration = DefaultConfiguration.Configuration;
            // It doesn't matter if we edit the static configuration in this program.
            configuration.Jobs = [];

            /* Adding the default configuration to the new database */
            myTimeClassifierDbContext.Configurations.Add(configuration);
            myTimeClassifierDbContext.SaveChanges();

            /* We make sure to get the attached configuration from the database now */
            configuration = myTimeClassifierDbContext.Configurations.First();

            /* We make sure to get all the task's job names from the task list, instead of the job list, because the job list might not be complete */
            var allJobName = odlDbContext.Tasks.Select(x => x.JobName)
                .Distinct()
                .ToHashSet();

            /* We make sure to get the currently used jobs that might not have been used yet in the old database */
            foreach (var oldJob in odlDbContext.JobList)
                allJobName.Add(oldJob.nameTache);

            /* Custom check for actual duplicated jobs */
            // Check if allJobName has any duplicates, which mean -> 8 first characters are the same
            var duplicatedGroup = allJobName
                .Where(x => x?.Length > 7)
                .GroupBy(x => x?.Substring(0, 8) ?? string.Empty)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

            List<string?> allJobNameFull = [..allJobName];

            /* Remove all but the longest duplicate of each group from the AllJobName list */
            foreach (var duplicateStart in duplicatedGroup)
            {
                var similarDuplicates = allJobName
                    .Where(x => x?.StartsWith(duplicateStart) ?? false)
                    .OrderByDescending(x => x?.Length)
                    .ToArray();

                var longestDuplicate = similarDuplicates.First();

                allJobName.RemoveWhere(x => similarDuplicates.Contains(x) && x != longestDuplicate);
            }

            var priority = 0u;

            /* Adding the Jobs to the configuration */
            foreach (var oldJobName in allJobName)
                configuration.Jobs.Add(new Job
                {
                    Text = oldJobName ?? string.Empty,
                    Priority = priority++,
                    Enabled = true
                });

            /* Adding the Jobs to the DB (so we can then use their IDs to link the tasks to them) */
            myTimeClassifierDbContext.SaveChanges();
            Console.WriteLine("Jobs imported, now importing tasks...");

            /* For performance reasons, we will use a dictionary to link the job names to their IDs */
            var nameToJobID = myTimeClassifierDbContext.Jobs.ToDictionary(x => x.Text, x => x.Id);

            /* Adding back the duplicated jobs to put to their homologue longer duplicate */
            foreach (var duplicateStart in duplicatedGroup)
            {
                var similarDuplicates = allJobNameFull
                    .Where(x => x?.StartsWith(duplicateStart) ?? false)
                    .OrderByDescending(x => x?.Length)
                    .ToArray();

                var longestDuplicate = similarDuplicates.First();

                if (longestDuplicate is null) continue;

                similarDuplicates
                    .Where(x => x != longestDuplicate).ToList()
                    .ForEach(x =>
                    {
                        if (x is null) return;

                        nameToJobID.Add(x, nameToJobID[longestDuplicate]);
                    });
            }

            uint autoIncrement = 1;

            foreach (var oldTask in odlDbContext.Tasks)
            {
                if (oldTask.JobName == null)
                    continue;

                var currentJobID = nameToJobID[oldTask.JobName];
                /* We divide by 1000 to convert the UnixTime from milliseconds to seconds */
                myTimeClassifierDbContext.Tasks.Add(new Task
                {
                    Id = autoIncrement++,
                    JobID = currentJobID,
                    UnixStartTime = (uint?)(oldTask.dateD / 1000) ?? 0,
                    UnixEndTime = (uint?)(oldTask.dateF / 1000) ?? 0
                });
            }

            myTimeClassifierDbContext.SaveChanges();

            /* Adding default colors to the jobs that were in the old database but selected */
            var oldSelectedJobs = odlDbContext.SelectedJobs.ToArray();

            for (var i = 0; i < oldSelectedJobs.Length; i++)
            {
                var oldSelectedJob = oldSelectedJobs[i];
                var currentJob = configuration.Jobs.FirstOrDefault(x
                    => x.Id == nameToJobID[oldSelectedJob.nameTache ?? string.Empty]);
                if (currentJob is null) continue;

                currentJob.FillColor = DefaultConfiguration.Configuration
                    .Jobs[i % DefaultConfiguration.Configuration.Jobs.Count].FillColor;
                currentJob.StrokeColor = DefaultConfiguration.Configuration
                    .Jobs[i % DefaultConfiguration.Configuration.Jobs.Count].StrokeColor;
                currentJob.ContentColor = DefaultConfiguration.Configuration
                    .Jobs[i % DefaultConfiguration.Configuration.Jobs.Count].ContentColor;
            }

            myTimeClassifierDbContext.SaveChanges();

            Console.WriteLine("Tasks imported successfully!");
            Console.WriteLine(
                $"The new database has been placed in the directory of this application, under the name '{AppDbContext.DatabasePathName}'");
            ConsoleInputUtils.PressAnyKeyToExit();
        }
        catch (Exception exception)
        {
            Console.WriteLine("Something went wrong: {0}", exception);
            ConsoleInputUtils.PressAnyKeyToExit();
        }
    }
}