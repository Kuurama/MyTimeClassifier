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

    private void DeleteButton_Click(object p_Sender, RoutedEventArgs _)
    {
        if (p_Sender is not Button { Parent: StackPanel l_StackPanel } l_Button)
            return;

        l_Button.IsVisible = false;
        /* Show the ConfirmDeletePanel */
        l_StackPanel.Children[1].IsVisible = true;
    }

    private void CloseDeleteMenuButton_Click(object p_Sender, RoutedEventArgs _)
    {
        if (p_Sender is not Button { Parent: StackPanel { Parent: StackPanel l_ParentStackPanel } l_StackPanel })
            return;

        /* Hide the ConfirmDeletePanel and show the DeleteButton */
        l_StackPanel.IsVisible                   = false;
        l_ParentStackPanel.Children[0].IsVisible = true;
    }
}
