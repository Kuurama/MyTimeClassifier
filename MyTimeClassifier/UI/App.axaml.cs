using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using MyTimeClassifier.UI.Views;

namespace MyTimeClassifier.UI;

public class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime l_Desktop)
            l_Desktop.MainWindow = new MainWindow();

        base.OnFrameworkInitializationCompleted();
    }

    public void ChangeTheme(ThemeVariant p_ThemeVariant) => RequestedThemeVariant = p_ThemeVariant;
}
