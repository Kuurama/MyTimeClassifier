using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using MyTimeClassifier.UI.Models;
using MyTimeClassifier.UI.ViewModels;
using MyTimeClassifier.Utils;

namespace MyTimeClassifier.UI.Views;

public partial class HistoryWindow : Window
{
    public HistoryWindow() => InitializeComponent();

    protected override void OnPointerPressed(PointerPressedEventArgs args) => WindowHelper.Drag(this, args);

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        if (DataContext is IDisposable disposable)
            disposable.Dispose();
    }

    public void OnCloseButton(object? sender, RoutedEventArgs _) => WindowHelper.CloseButton_Click(this);

    private void OnContextMenuButtonPressed(object? sender, RoutedEventArgs _)
    {
        if (sender is not Button { ContextMenu: not null } textBox)
            return;

        textBox.ContextMenu.Open(textBox);
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs _)
    {
        if (sender is not Button { Parent: StackPanel stackPanel } button)
            return;

        button.IsVisible = false;

        /* Show the ConfirmDeletePanel */
        stackPanel.Children[1].IsVisible = true;
    }

    private void ConfirmDeleteButton_Click(object sender, RoutedEventArgs _)
    {
        if (sender is not Button { Parent: StackPanel { Parent: StackPanel parentStackPanel } stackPanel })
            return;

        /* Hide the ConfirmDeletePanel and show the DeleteButton */
        stackPanel.IsVisible = false;
        parentStackPanel.Children[0].IsVisible = true;

        if (DataContext is not HistoryWindowViewModel viewModel) return;
        if (stackPanel.DataContext is not TaskModel taskModel) return;

        /* Tells the UI to wait until either the Task has been deleted, or if 5 set passed */
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var timeout = 0;

            while (taskModel.TaskExists && timeout < 5000)
            {
                await Task.Delay(10);
                timeout += 10;
            }

            /* Task was not deleted within the 5 sec mark */
            if (timeout >= 5000)
                return;

            /* Reload the list data (since deletion occured) */
            viewModel.ReLoadData();
        }, DispatcherPriority.Background);
    }

    private void CloseDeleteMenuButton_Click(object sender, RoutedEventArgs _)
    {
        if (sender is not Button { Parent: StackPanel { Parent: StackPanel parentStackPanel } stackPanel })
            return;

        /* Hide the ConfirmDeletePanel and show the DeleteButton */
        stackPanel.IsVisible = false;
        parentStackPanel.Children[0].IsVisible = true;
    }
}