using Avalonia.Media;
using MyTimeClassifier.Database.Entities;

namespace MyTimeClassifier.Configuration;

public static class DefaultConfiguration
{
    public const uint AutoSaveIntervalSeconds = 60 * 10;
    public const float MinimumGlobalScale = 0.2f;
    public const float MaximumGlobalScale = 2.0f;

    public static readonly Database.Entities.Configuration Configuration = new()
    {
        Id = 0,
        ProgramName = "MyTimeClassifier",
        UseLightTheme = false,
        RadialSelectorRadius = 450,
        RadialContentScale = 1.0f,
        GlobalScale = 0.7f,
        IsMinimalistic = false,
        InnerRadiusRatio = 0.5f,
        SpacingAngle = 30,
        TimerScale = 1.25f,
        TitleBarScale = 1.0f,
        Jobs =
        [
            new Job
            {
                Text = "Reading",
                Emoji = "fa-book-open",
                FillColor = new SolidColorBrush(Color.Parse("#ffff57")),
                StrokeColor = new SolidColorBrush(Color.Parse("#b89f56")),
                ContentColor = new SolidColorBrush(Color.Parse("#2D2305")),
                Priority = 1,
                Enabled = true
            },
            new Job
            {
                Text = "Teaching",
                Emoji = "fa-chalkboard-teacher",
                FillColor = new SolidColorBrush(Color.Parse("#7d66d9")),
                StrokeColor = new SolidColorBrush(Color.Parse("#56468b")),
                ContentColor = new SolidColorBrush(Color.Parse("#291F43")),
                Priority = 2,
                Enabled = true
            },
            new Job
            {
                Text = "Working",
                Emoji = "fa-laptop-code",
                FillColor = new SolidColorBrush(Color.Parse("#ec6142")),
                StrokeColor = new SolidColorBrush(Color.Parse("#ac4d39")),
                ContentColor = new SolidColorBrush(Color.Parse("#391714")),
                Priority = 3,
                Enabled = true
            },
            new Job
            {
                Text = "Movies",
                Emoji = "fa-photo-film",
                FillColor = new SolidColorBrush(Color.Parse("#ae8c7e")),
                StrokeColor = new SolidColorBrush(Color.Parse("#6f5f58")),
                ContentColor = new SolidColorBrush(Color.Parse("#262220")),
                Priority = 4,
                Enabled = true
            },
            new Job
            {
                Text = "Meeting",
                Emoji = "fa-people-group",
                FillColor = new SolidColorBrush(Color.Parse("#b658c4")),
                StrokeColor = new SolidColorBrush(Color.Parse("#92549c")),
                ContentColor = new SolidColorBrush(Color.Parse("#351A35")),
                Priority = 5,
                Enabled = true
            },
            new Job
            {
                Text = "Coffee",
                Emoji = "fa-coffee",
                FillColor = new SolidColorBrush(Color.Parse("#717d79")),
                StrokeColor = new SolidColorBrush(Color.Parse("#5b625f")),
                ContentColor = new SolidColorBrush(Color.Parse("#202221")),
                Priority = 6,
                Enabled = true
            },
            new Job
            {
                Text = "Preparing",
                Emoji = "fa-clipboard-list",
                FillColor = new SolidColorBrush(Color.Parse("#ec5a72")),
                StrokeColor = new SolidColorBrush(Color.Parse("#b3445a")),
                ContentColor = new SolidColorBrush(Color.Parse("#3A141E")),
                Priority = 7,
                Enabled = true
            },
            new Job
            {
                Text = "Chilling",
                Emoji = "fa-leaf",
                FillColor = new SolidColorBrush(Color.Parse("#33b074")),
                StrokeColor = new SolidColorBrush(Color.Parse("#2f7c57")),
                ContentColor = new SolidColorBrush(Color.Parse("#132D21")),
                Priority = 8,
                Enabled = true
            },
            new Job
            {
                Text = "Mailing",
                Emoji = "fa-mail-bulk",
                FillColor = new SolidColorBrush(Color.Parse("#ee518a")),
                StrokeColor = new SolidColorBrush(Color.Parse("#b0436e")),
                ContentColor = new SolidColorBrush(Color.Parse("#381525")),
                Priority = 9,
                Enabled = true
            }
        ]
    };
}