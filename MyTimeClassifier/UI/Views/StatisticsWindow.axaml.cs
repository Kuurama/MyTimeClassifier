using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MyTimeClassifier.UI.ViewModels;
using MyTimeClassifier.Utils;

namespace MyTimeClassifier.UI.Views;

public partial class StatisticsWindow : Window
{
    public StatisticsWindow()
    {
        InitializeComponent();

        var statisticWindowViewModel = new StatisticsWindowViewModel(Pie);
        DataContext = statisticWindowViewModel;
    }

    protected override void OnPointerPressed(PointerPressedEventArgs args) => WindowHelper.Drag(this, args);

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        if (DataContext is IDisposable disposable)
            disposable.Dispose();
    }

    public void OnCloseButton(object? sender, RoutedEventArgs _) => WindowHelper.CloseButton_Click(this);
}