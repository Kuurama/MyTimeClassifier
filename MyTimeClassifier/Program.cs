using Avalonia;
using Avalonia.ReactiveUI;
using MyTimeClassifier.UI;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using System;
using System.Threading;

namespace MyTimeClassifier;

internal sealed class Program
{
    private const string MutexName = "MyTimeClassifierMutex";
    public static Mutex _mutex = null!;
    
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] p_Args)
    {
        /* Mutex might only work on Windows */
        _mutex = new Mutex(true, MutexName, out var l_IsNewInstance);
        if (!l_IsNewInstance && !_mutex.WaitOne(1000))
        {
            _mutex.Dispose();
            return;
        }

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(p_Args);
    }

    private static AppBuilder BuildAvaloniaApp()
    {
        IconProvider.Current.Register<FontAwesomeIconProvider>();
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
    }
}
