using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Threading;
using MyTimeClassifier.Configuration;
using MyTimeClassifier.UI.ViewModels;
using MyTimeClassifier.Utils;

namespace MyTimeClassifier.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        /* Register the UI related events into the DataContext */
        DataContext = new MainWindowViewModel(JobSelector);
        this.FindControl<Thumb>("ResizeGrip")?.AddHandler(Thumb.DragDeltaEvent, ResizeGrip_OnDragDelta);

        ((App?)Application.Current)?.ChangeTheme(AppConfiguration.StaticCache.UseLightTheme
            ? ThemeVariant.Light
            : ThemeVariant.Dark);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs args) => WindowHelper.Drag(this, args);

    public void OnMinimizeButton(object? sender, RoutedEventArgs _) => WindowHelper.MinimizeButton_Click(this);

    public void OnCloseButton(object? sender, RoutedEventArgs _) => WindowHelper.CloseButton_Click(this);

    private void OnStatsButton(object? sender, RoutedEventArgs _)
        => new StatisticsWindow().ShowDialog(this);

    private void OnSettingsButton(object? sender, RoutedEventArgs _)
        => new SettingsWindow { DataContext = new SettingsWindowViewModel() }.ShowDialog(this);

    private void OnHistoryButton(object? pSender, RoutedEventArgs _)
        => new HistoryWindow { DataContext = new HistoryWindowViewModel() }.ShowDialog(this);


    /// <summary>
    /// Event handler for the resize grip.
    /// Changes the GlobalScore of the application.
    /// Makes sure that the scale is within the bounds.
    /// </summary>
    /// <param name="_"></param>
    /// <param name="e"></param>
    private void ResizeGrip_OnDragDelta(object? _, VectorEventArgs e)
    {
        if (WindowState != WindowState.Normal)
            return;

        var config = AppConfiguration.StaticCache;

        // Ensure that the steps are not too big (to avoid UI Flickering).
        var newScale = config.GlobalScale + Math.Sign(e.Vector.X) * (Math.Min(Math.Abs(e.Vector.X), 1) / 100);

        // Ensure that the scale is within the bounds.
        newScale = Math.Max(DefaultConfiguration.MinimumGlobalScale,
            Math.Min(DefaultConfiguration.MaximumGlobalScale, newScale));

        if (Math.Abs(config.GlobalScale - newScale) < 0.01)
            return;

        Dispatcher.UIThread.InvokeAsync(() => { config.GlobalScale = (float)newScale; });
    }
}