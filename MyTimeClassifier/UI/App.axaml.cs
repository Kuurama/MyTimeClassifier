using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using MyTimeClassifier.Configuration;
using MyTimeClassifier.UI.Views;

namespace MyTimeClassifier.UI;

public class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime l_Desktop)
        {
            l_Desktop.MainWindow   = new MainWindow();
            l_Desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;

            /* Ensure that the settings are saved when the application is closed (even when force closed with alt+F4) */
            l_Desktop.ShutdownRequested += (_, _) =>
            {
                AppConfiguration.SaveConfiguration();
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    public void ChangeTheme(ThemeVariant p_ThemeVariant) => RequestedThemeVariant = p_ThemeVariant;
}
