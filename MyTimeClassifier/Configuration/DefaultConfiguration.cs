using Avalonia.Media;
using MyTimeClassifier.Database.Entities;
using System.Collections.Generic;

namespace MyTimeClassifier.Configuration;

public static class DefaultConfiguration
{
    /*(Brushes.Red, Brushes.IndianRed),
       (Brushes.Green, Brushes.LightGreen),
       (Brushes.Blue, Brushes.LightBlue),
       (Brushes.Yellow, Brushes.LightYellow),
       (Brushes.Purple, Brushes.MediumPurple),
       (Brushes.Orange, Brushes.SandyBrown),
       (Brushes.Pink, Brushes.LightPink),
       (Brushes.Brown, Brushes.SandyBrown),
       (Brushes.Gray, Brushes.LightGray)*/

    public static readonly List<Job> s_Jobs =
    [
        new Job
        {
            FillColor = Brushes.Red
        },
        new Job
        {
            FillColor = Brushes.Green
        },
        new Job
        {
            FillColor = Brushes.Blue
        },
        new Job
        {
            FillColor = Brushes.Yellow
        },
        new Job
        {
            FillColor = Brushes.Purple
        },
        new Job
        {
            FillColor = Brushes.Orange
        },
        new Job
        {
            FillColor = Brushes.Pink
        },
        new Job
        {
            FillColor = Brushes.Brown
        },
        new Job
        {
            FillColor = Brushes.Gray
        }
    ];
}
