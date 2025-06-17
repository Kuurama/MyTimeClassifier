using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MyTimeClassifier.UI.ViewModels;
using MyTimeClassifier.Utils;

namespace MyTimeClassifier.UI.Views;

public partial class JobEditWindow : Window
{
    public JobEditWindow() => InitializeComponent();

    protected override void OnPointerPressed(PointerPressedEventArgs args) => WindowHelper.Drag(this, args);

    public void OnCloseButton(object? sender, RoutedEventArgs _) => WindowHelper.CloseButton_Click(this);

    private void EmojiTextBlock_PointerPressed(object? sender, PointerPressedEventArgs _)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "https://fontawesome.com/search",
            UseShellExecute = true
        };
        Process.Start(processStartInfo);
    }

    private void OnNewRowButton(object? sender, RoutedEventArgs _) => JobEditWindowViewModel.AddNewRow();

    private void DeleteButton_Click(object sender, RoutedEventArgs _)
    {
        if (sender is not Button { Parent: StackPanel stackPanel } button)
            return;

        button.IsVisible = false;
        /* Show the ConfirmDeletePanel */
        stackPanel.Children[1].IsVisible = true;
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