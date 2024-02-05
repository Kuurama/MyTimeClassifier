using Avalonia;
using Avalonia.Styling;
using MyTimeClassifier.Configuration;

namespace MyTimeClassifier.UI.ViewModels;

/// <summary>
///     This class will be in charge of updating the Configuration singleton and updating the UI accordingly
/// </summary>
public class SettingsWindowViewModel : ViewModelBase
{
    public SettingsWindowViewModel()
    {
        var l_Configuration = AppConfiguration.StaticCache;

        UseLightTheme = l_Configuration.UseLightTheme;
    }


    public bool UseLightTheme
    {
        get => AppConfiguration.StaticCache.UseLightTheme;
        set
        {
            AppConfiguration.StaticCache.UseLightTheme = value;
            // Change the theme of the application
            ((App?)Application.Current)?.ChangeTheme(AppConfiguration.StaticCache.UseLightTheme ? ThemeVariant.Light : ThemeVariant.Dark);
        }
    }
}
