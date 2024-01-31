using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MyTimeClassifier.Utils;

namespace MyTimeClassifier.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var l_RadialSelector = new RadialSelector(9, null, 450);
        Body.Children.Add(l_RadialSelector);
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    protected override void OnPointerPressed(PointerPressedEventArgs p_Args) => WindowHelper.Drag(this, p_Args);

    public void OnMinimizeButton(object? p_Sender, RoutedEventArgs _) => WindowHelper.MinimizeButton_Click(this);
    public void OnCloseButton(object?    p_Sender, RoutedEventArgs _) => WindowHelper.CloseButton_Click(this);
}
