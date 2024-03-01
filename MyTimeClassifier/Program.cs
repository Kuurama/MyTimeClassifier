using Avalonia;
using Avalonia.ReactiveUI;
using MyTimeClassifier.UI;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using System;

namespace MyTimeClassifier;

internal sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] p_Args)
    {
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(p_Args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        IconProvider.Current.Register<FontAwesomeIconProvider>();
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
    }
}
