using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MyTimeClassifier.UI.ViewModels;
using MyTimeClassifier.Utils;
using System.Diagnostics;

namespace MyTimeClassifier.UI.Views;

public partial class JobEditWindow : Window
{
    public JobEditWindow()
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

    private void EmojiTextBlock_PointerPressed(object? p_Sender, PointerPressedEventArgs _)
    {
        var l_Psi = new ProcessStartInfo
        {
            FileName        = "https://fontawesome.com/search",
            UseShellExecute = true
        };
        Process.Start(l_Psi);
    }

    private void OnNewRowButton(object? p_Sender, RoutedEventArgs _)
    {
        /* Add a new row to the list */
        JobEditWindowViewModel.AddNewRow();
    }
}
