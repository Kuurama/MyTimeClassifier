using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MyTimeClassifier.Utils;

namespace MyTimeClassifier.UI.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    protected override void OnPointerPressed(PointerPressedEventArgs p_Args) => WindowHelper.Drag(this, p_Args);

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public void OnCloseButton(object? p_Sender, RoutedEventArgs _) => WindowHelper.CloseButton_Click(this);
}
