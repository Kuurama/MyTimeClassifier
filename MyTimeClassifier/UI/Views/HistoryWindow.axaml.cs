using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MyTimeClassifier.Utils;

namespace MyTimeClassifier.UI.Views;

public class HistoryWindow : Window
{
    public HistoryWindow()
    {
        InitializeComponent();
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    protected override void OnPointerPressed(PointerPressedEventArgs p_Args) => WindowHelper.Drag(this, p_Args);

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public void OnCloseButton(object? p_Sender, RoutedEventArgs _)
    {
        /* Close the window */
        WindowHelper.CloseButton_Click(this);
    }
}
