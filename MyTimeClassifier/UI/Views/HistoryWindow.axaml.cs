using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using MyTimeClassifier.UI.Models;
using MyTimeClassifier.UI.ViewModels;
using MyTimeClassifier.Utils;
using System.Threading.Tasks;

namespace MyTimeClassifier.UI.Views;

public partial class HistoryWindow : Window
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

    private void OnContextMenuButtonPressed(object? p_Sender, RoutedEventArgs _)
    {
        if (p_Sender is not Button { ContextMenu: not null } l_TextBox)
            return;

        l_TextBox.ContextMenu.Open(l_TextBox);
    }

    private void DeleteButton_Click(object p_Sender, RoutedEventArgs _)
    {
        if (p_Sender is not Button { Parent: StackPanel l_StackPanel } l_Button)
            return;

        l_Button.IsVisible = false;
        /* Show the ConfirmDeletePanel */
        l_StackPanel.Children[1].IsVisible = true;
    }

    private void ConfirmDeleteButton_Click(object p_Sender, RoutedEventArgs _)
    {
        if (p_Sender is not Button { Parent: StackPanel { Parent: StackPanel l_ParentStackPanel } l_StackPanel })
            return;

        /* Hide the ConfirmDeletePanel and show the DeleteButton */
        l_StackPanel.IsVisible                   = false;
        l_ParentStackPanel.Children[0].IsVisible = true;

        if (DataContext is not HistoryWindowViewModel l_ViewModel) return;
        if (l_StackPanel.DataContext is not TaskModel l_TaskModel) return;

        /* Tells the UI to wait until either the Task has been deleted, or if 5 set passed */
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var l_Timeout = 0;

            while (l_TaskModel.TaskExists && l_Timeout < 5000)
            {
                await Task.Delay(10);
                l_Timeout += 10;
            }

            /* Task was not deleted within the 5 sec mark */
            if (l_Timeout >= 5000)
                return;

            /* Reload the list data (since deletion occured) */
            l_ViewModel.ReLoadData();
        }, DispatcherPriority.Background);
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
