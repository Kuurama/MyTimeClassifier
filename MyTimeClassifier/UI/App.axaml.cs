using System;
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
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
            desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;

            /* Ensure that the settings are saved when the application is closed (even when force closed with alt+F4) */
            desktop.ShutdownRequested += (_, _) =>
            {
                AppConfiguration.SaveConfiguration();

                Program._mutex.ReleaseMutex();
                Program._mutex.Dispose();
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    public void ChangeTheme(ThemeVariant themeVariant) => RequestedThemeVariant = themeVariant;

    private void NativeMenuItem_Show_OnClick(object? sender, EventArgs _)
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return;

        desktop.MainWindow ??= new MainWindow();
        desktop.MainWindow.Show();
    }

    private void NativeMenuItem_Exit_OnClick(object? sender, EventArgs _)
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return;

        desktop.MainWindow?.Close();
    }
}