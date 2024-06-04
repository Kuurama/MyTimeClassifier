using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using MyTimeClassifier.Configuration;
using MyTimeClassifier.UI.Views;
using System;

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
                
                Program._mutex.ReleaseMutex();
                Program._mutex.Dispose();
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    public void ChangeTheme(ThemeVariant p_ThemeVariant) => RequestedThemeVariant = p_ThemeVariant;

    private void NativeMenuItem_Show_OnClick(object? p_Sender, EventArgs _)
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime l_Desktop)
            return;

        l_Desktop.MainWindow ??= new MainWindow();
        l_Desktop.MainWindow.Show();
    }

    private void NativeMenuItem_Exit_OnClick(object? p_Sender, EventArgs _)
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime l_Desktop)
            return;

        l_Desktop.MainWindow?.Close();
    }
}
