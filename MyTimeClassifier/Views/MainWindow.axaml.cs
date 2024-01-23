using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using MyTimeClassifier.Utils;

namespace MyTimeClassifier.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var l_RadialSelector = new RadialSelector(8, 100);
        Body.Children.Add(l_RadialSelector);

        /*Body.Children.Add(new Rectangle
        {
            Width = 100,
            Height = 100,
            Fill = Brushes.Red,
            HorizontalAlignment = (HorizontalAlignment)2,
            VerticalAlignment = (VerticalAlignment)2,
            RadiusX = 35,
            RadiusY = -40

        });*/
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    protected override void OnPointerPressed(PointerPressedEventArgs p_Args) => WindowHelper.Drag(this, p_Args);
    private void OnMinimizeButton(object? p_Sender, RoutedEventArgs p_E) => WindowHelper.MinimizeButton_Click(this);
    private void OnCloseButton(object? p_Sender, RoutedEventArgs p_E) => WindowHelper.CloseButton_Click(this);
}
