using Avalonia.Media;
using MyTimeClassifier.Database.Entities;
using System.Collections.ObjectModel;

namespace MyTimeClassifier.Configuration;

public static class DefaultConfiguration
{
    public static readonly ObservableCollection<Job> s_Jobs =
    [
        new Job
        {
            Id           = new Job.JobID(1),
            Text         = "Reading",
            Emoji        = "fa-solid fa-book-open",
            FillColor    = new SolidColorBrush(Color.Parse("#ffff57")),
            StrokeColor  = new SolidColorBrush(Color.Parse("#b89f56")),
            ContentColor = new SolidColorBrush(Color.Parse("#2D2305"))
        },
        new Job
        {
            Id           = new Job.JobID(2),
            Text         = "Teaching",
            Emoji        = "fa-solid fa-chalkboard-teacher",
            FillColor    = new SolidColorBrush(Color.Parse("#7d66d9")),
            StrokeColor  = new SolidColorBrush(Color.Parse("#56468b")),
            ContentColor = new SolidColorBrush(Color.Parse("#291F43"))
        },
        new Job
        {
            Id           = new Job.JobID(3),
            Text         = "Working",
            Emoji        = "fa-solid fa-laptop-code",
            FillColor    = new SolidColorBrush(Color.Parse("#ec6142")),
            StrokeColor  = new SolidColorBrush(Color.Parse("#ac4d39")),
            ContentColor = new SolidColorBrush(Color.Parse("#391714"))
        },
        new Job
        {
            Id           = new Job.JobID(4),
            Text         = "Movies",
            Emoji        = "fa-solid fa-photo-film",
            FillColor    = new SolidColorBrush(Color.Parse("#ae8c7e")),
            StrokeColor  = new SolidColorBrush(Color.Parse("#6f5f58")),
            ContentColor = new SolidColorBrush(Color.Parse("#262220"))
        },
        new Job
        {
            Id           = new Job.JobID(5),
            Text         = "Meeting",
            Emoji        = "fa-solid fa-people-group",
            FillColor    = new SolidColorBrush(Color.Parse("#b658c4")),
            StrokeColor  = new SolidColorBrush(Color.Parse("#92549c")),
            ContentColor = new SolidColorBrush(Color.Parse("#351A35"))
        },
        new Job
        {
            Id           = new Job.JobID(6),
            Text         = "Coffee",
            Emoji        = "fa-solid fa-coffee",
            FillColor    = new SolidColorBrush(Color.Parse("#717d79")),
            StrokeColor  = new SolidColorBrush(Color.Parse("#5b625f")),
            ContentColor = new SolidColorBrush(Color.Parse("#202221"))
        },
        new Job
        {
            Id           = new Job.JobID(7),
            Text         = "Preparing",
            Emoji        = "fa-solid fa-clipboard-list",
            FillColor    = new SolidColorBrush(Color.Parse("#ec5a72")),
            StrokeColor  = new SolidColorBrush(Color.Parse("#b3445a")),
            ContentColor = new SolidColorBrush(Color.Parse("#3A141E"))
        },
        new Job
        {
            Id           = new Job.JobID(8),
            Text         = "Chilling",
            Emoji        = "fa-solid fa-leaf",
            FillColor    = new SolidColorBrush(Color.Parse("#33b074")),
            StrokeColor  = new SolidColorBrush(Color.Parse("#2f7c57")),
            ContentColor = new SolidColorBrush(Color.Parse("#132D21"))
        },
        new Job
        {
            Id           = new Job.JobID(9),
            Text         = "Mailing",
            Emoji        = "fa-solid fa-mail-bulk",
            FillColor    = new SolidColorBrush(Color.Parse("#ee518a")),
            StrokeColor  = new SolidColorBrush(Color.Parse("#b0436e")),
            ContentColor = new SolidColorBrush(Color.Parse("#381525"))
        }
    ];
}
