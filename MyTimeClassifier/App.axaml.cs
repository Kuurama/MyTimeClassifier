using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MyTimeClassifier.ViewModels;
using MyTimeClassifier.Views;

namespace MyTimeClassifier;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime l_Desktop)
            l_Desktop.MainWindow = new MainWindow { DataContext = new MainWindowViewModel() };

        base.OnFrameworkInitializationCompleted();
    }
}
