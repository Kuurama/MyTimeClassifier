using Avalonia.Media;
using MyTimeClassifier.Database.Entities;

namespace MyTimeClassifier.Configuration;

public static class DefaultConfiguration
{
    public const uint AutoSaveIntervalSeconds = 60 * 10;
    public const float MinimumGlobalScale = 0.2f;
    public const float MaximumGlobalScale = 2.0f;

    public static readonly Database.Entities.Configuration Configuration = new(
        id: 0,
        programName: "MyTimeClassifier",
        useLightTheme: false,
        radialSelectorRadius: 450,
        radialContentScale: 1.0f,
        globalScale: 0.7f,
        isMinimalistic: false,
        innerRadiusRatio: 0.5f,
        spacingAngle: 30,
        timerScale: 1.25f,
        titleBarScale: 1.0f,
        jobs:
        [
            new Job(
                text: "Reading",
                emoji: "fa-book-open",
                fillColor: new SolidColorBrush(Color.Parse("#ffff57")),
                strokeColor: new SolidColorBrush(Color.Parse("#b89f56")),
                contentColor: new SolidColorBrush(Color.Parse("#2D2305")),
                priority: 1,
                isRadial: true,
                enabled: true
            ),
            new Job(
                text: "Teaching",
                emoji: "fa-chalkboard-teacher",
                fillColor: new SolidColorBrush(Color.Parse("#7d66d9")),
                strokeColor: new SolidColorBrush(Color.Parse("#56468b")),
                contentColor: new SolidColorBrush(Color.Parse("#291F43")),
                priority: 2,
                isRadial: true,
                enabled: true
            ),
            new Job(
                text: "Working",
                emoji: "fa-laptop-code",
                fillColor: new SolidColorBrush(Color.Parse("#ec6142")),
                strokeColor: new SolidColorBrush(Color.Parse("#ac4d39")),
                contentColor: new SolidColorBrush(Color.Parse("#391714")),
                priority: 3,
                isRadial: true,
                enabled: true
            ),
            new Job(
                text: "Movies",
                emoji: "fa-photo-film",
                fillColor: new SolidColorBrush(Color.Parse("#ae8c7e")),
                strokeColor: new SolidColorBrush(Color.Parse("#6f5f58")),
                contentColor: new SolidColorBrush(Color.Parse("#262220")),
                priority: 4,
                isRadial: true,
                enabled: true
            ),
            new Job(
                text: "Meeting",
                emoji: "fa-people-group",
                fillColor: new SolidColorBrush(Color.Parse("#b658c4")),
                strokeColor: new SolidColorBrush(Color.Parse("#92549c")),
                contentColor: new SolidColorBrush(Color.Parse("#351A35")),
                priority: 5,
                isRadial: true,
                enabled: true
            ),
            new Job(
                text: "Coffee",
                emoji: "fa-coffee",
                fillColor: new SolidColorBrush(Color.Parse("#717d79")),
                strokeColor: new SolidColorBrush(Color.Parse("#5b625f")),
                contentColor: new SolidColorBrush(Color.Parse("#202221")),
                priority: 6,
                isRadial: true,
                enabled: true
            ),
            new Job(
                text: "Preparing",
                emoji: "fa-clipboard-list",
                fillColor: new SolidColorBrush(Color.Parse("#ec5a72")),
                strokeColor: new SolidColorBrush(Color.Parse("#b3445a")),
                contentColor: new SolidColorBrush(Color.Parse("#3A141E")),
                priority: 7,
                isRadial: true,
                enabled: true
            ),
            new Job(
                text: "Chilling",
                emoji: "fa-leaf",
                fillColor: new SolidColorBrush(Color.Parse("#33b074")),
                strokeColor: new SolidColorBrush(Color.Parse("#2f7c57")),
                contentColor: new SolidColorBrush(Color.Parse("#132D21")),
                priority: 8,
                isRadial: true,
                enabled: true
            ),
            new Job(
                text: "Mailing",
                emoji: "fa-mail-bulk",
                fillColor: new SolidColorBrush(Color.Parse("#ee518a")),
                strokeColor: new SolidColorBrush(Color.Parse("#b0436e")),
                contentColor: new SolidColorBrush(Color.Parse("#381525")),
                priority: 9,
                isRadial: true,
                enabled: true
            )
        ]
    );
}