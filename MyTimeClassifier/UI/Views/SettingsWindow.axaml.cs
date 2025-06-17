using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MyTimeClassifier.Configuration;
using MyTimeClassifier.UI.ViewModels;
using MyTimeClassifier.Utils;

namespace MyTimeClassifier.UI.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow() => InitializeComponent();

    protected override void OnPointerPressed(PointerPressedEventArgs args) => WindowHelper.Drag(this, args);

    public void OnCloseButton(object? sender, RoutedEventArgs _)
    {
        AppConfiguration.SaveConfiguration();
        WindowHelper.CloseButton_Click(this);
    }

    private void OnJobsEdit(object? sender, RoutedEventArgs _)
        => new JobEditWindow { DataContext = new JobEditWindowViewModel() }.ShowDialog(this);
}