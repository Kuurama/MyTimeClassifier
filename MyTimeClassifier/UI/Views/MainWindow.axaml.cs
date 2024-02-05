using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MyTimeClassifier.UI.ViewModels;
using MyTimeClassifier.Utils;

namespace MyTimeClassifier.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        /* Register the UI related events into the DataContext */
        DataContext = new MainWindowViewModel(JobSelector);
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    protected override void OnPointerPressed(PointerPressedEventArgs p_Args) => WindowHelper.Drag(this, p_Args);

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public void OnMinimizeButton(object? p_Sender, RoutedEventArgs _) => WindowHelper.MinimizeButton_Click(this);
    public void OnCloseButton(object?    p_Sender, RoutedEventArgs _) => WindowHelper.CloseButton_Click(this);

    public void OnSettingsButton(object? p_Sender, RoutedEventArgs _)
    {
        var l_SettingsWindow = new SettingsWindow { DataContext = new SettingsWindowViewModel() };
        l_SettingsWindow.ShowDialog(this);
    }
}
