using Avalonia.Media;
using MyTimeClassifier.Database.Entities;

namespace MyTimeClassifier.Configuration;

public static class DefaultConfiguration
{
    public static readonly float s_MinimumGlobalScale = 0.2f;
    public static readonly float s_MaximumGlobalScale = 2.0f;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public static readonly Database.Entities.Configuration s_Configuration = new
    (
        "MyTimeClassifier",
        450,
        0.5f,
        false,
        false,
        30,
        1.0f,
        0.7f,
        1.45f,
        1.0f,
        [
            new Job
            (
                new Job.JobID(1),
                "Reading",
                "fa-solid fa-book-open",
                new SolidColorBrush(Color.Parse("#ffff57")),
                new SolidColorBrush(Color.Parse("#b89f56")),
                new SolidColorBrush(Color.Parse("#2D2305"))
            ),
            new Job
            (
                new Job.JobID(2),
                "Teaching",
                "fa-solid fa-chalkboard-teacher",
                new SolidColorBrush(Color.Parse("#7d66d9")),
                new SolidColorBrush(Color.Parse("#56468b")),
                new SolidColorBrush(Color.Parse("#291F43"))
            ),
            new Job
            (
                new Job.JobID(3),
                "Working",
                "fa-solid fa-laptop-code",
                new SolidColorBrush(Color.Parse("#ec6142")),
                new SolidColorBrush(Color.Parse("#ac4d39")),
                new SolidColorBrush(Color.Parse("#391714"))
            ),
            new Job
            (
                new Job.JobID(4),
                "Movies",
                "fa-solid fa-photo-film",
                new SolidColorBrush(Color.Parse("#ae8c7e")),
                new SolidColorBrush(Color.Parse("#6f5f58")),
                new SolidColorBrush(Color.Parse("#262220"))
            ),
            new Job
            (
                new Job.JobID(5),
                "Meeting",
                "fa-solid fa-people-group",
                new SolidColorBrush(Color.Parse("#b658c4")),
                new SolidColorBrush(Color.Parse("#92549c")),
                new SolidColorBrush(Color.Parse("#351A35"))
            ),
            new Job
            (
                new Job.JobID(6),
                "Coffee",
                "fa-solid fa-coffee",
                new SolidColorBrush(Color.Parse("#717d79")),
                new SolidColorBrush(Color.Parse("#5b625f")),
                new SolidColorBrush(Color.Parse("#202221"))
            ),
            new Job
            (
                new Job.JobID(7),
                "Preparing",
                "fa-solid fa-clipboard-list",
                new SolidColorBrush(Color.Parse("#ec5a72")),
                new SolidColorBrush(Color.Parse("#b3445a")),
                new SolidColorBrush(Color.Parse("#3A141E"))
            ),
            new Job
            (
                new Job.JobID(8),
                "Chilling",
                "fa-solid fa-leaf",
                new SolidColorBrush(Color.Parse("#33b074")),
                new SolidColorBrush(Color.Parse("#2f7c57")),
                new SolidColorBrush(Color.Parse("#132D21"))
            ),
            new Job
            (
                new Job.JobID(9),
                "Mailing",
                "fa-solid fa-mail-bulk",
                new SolidColorBrush(Color.Parse("#ee518a")),
                new SolidColorBrush(Color.Parse("#b0436e")),
                new SolidColorBrush(Color.Parse("#381525"))
            )
        ]);
}
