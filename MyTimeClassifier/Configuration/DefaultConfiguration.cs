using Avalonia.Media;
using MyTimeClassifier.Database.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyTimeClassifier.Configuration;

public static class DefaultConfiguration
{
    public static readonly ObservableCollection<Job> s_Jobs =
    [
        new Job
        {
            Text      = "Reading",
            Emoji     = "fa-solid fa-book-open",
            FillColor = new SolidColorBrush(Color.Parse("#ffff57")),
            StrokeColor = new SolidColorBrush(Color.Parse("#b89f56"))
        },
        new Job
        {
            Text        = "Teaching",
            Emoji       = "fa-solid fa-chalkboard-teacher",
            FillColor   = new SolidColorBrush(Color.Parse("#7d66d9")),
            StrokeColor = new SolidColorBrush(Color.Parse("#56468b"))
        },
        new Job
        {
            Text        = "Working",
            Emoji       = "fa-solid fa-laptop-code",
            FillColor   = new SolidColorBrush(Color.Parse("#ec6142")),
            StrokeColor = new SolidColorBrush(Color.Parse("#ac4d39"))
        },
        new Job
        {
            Text        = "Movies",
            Emoji       = "fa-solid fa-photo-film",
            FillColor   = new SolidColorBrush(Color.Parse("#ae8c7e")),
            StrokeColor = new SolidColorBrush(Color.Parse("#6f5f58"))
        },
        new Job
        {
            Text        = "Meeting",
            Emoji       = "fa-solid fa-handshake",
            FillColor   = new SolidColorBrush(Color.Parse("#b658c4")),
            StrokeColor = new SolidColorBrush(Color.Parse("#92549c"))
        },
        new Job
        {
            Text        = "Coffee",
            Emoji       = "fa-solid fa-coffee",
            FillColor   = new SolidColorBrush(Color.Parse("#717d79")),
            StrokeColor = new SolidColorBrush(Color.Parse("#5b625f"))
        },
        new Job
        {
            Text        = "Preparing",
            Emoji       = "fa-solid fa-clipboard-list",
            FillColor   = new SolidColorBrush(Color.Parse("#ec5a72")),
            StrokeColor = new SolidColorBrush(Color.Parse("#b3445a"))
        },
        new Job
        {
            Text        = "Chilling",
            Emoji       = "fa-solid fa-leaf",
            FillColor   = new SolidColorBrush(Color.Parse("#33b074")),
            StrokeColor = new SolidColorBrush(Color.Parse("#2f7c57"))
        },
        new Job
        {
            Text        = "Mailing",
            Emoji       = "fa-solid fa-mail-bulk",
            FillColor   = new SolidColorBrush(Color.Parse("#ee518a")),
            StrokeColor = new SolidColorBrush(Color.Parse("#b0436e"))
        }
    ];
}
