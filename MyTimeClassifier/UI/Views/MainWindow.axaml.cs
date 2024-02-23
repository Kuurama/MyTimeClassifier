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
using System;

namespace MyTimeClassifier.UI.Views;

public class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        /* Register the UI related events into the DataContext */
        DataContext = new MainWindowViewModel(JobSelector);
        this.FindControl<Thumb>("ResizeGrip")?.AddHandler(Thumb.DragDeltaEvent, ResizeGrip_OnDragDelta);

        ((App?)Application.Current)?.ChangeTheme(AppConfiguration.StaticCache.UseLightTheme ? ThemeVariant.Light : ThemeVariant.Dark);
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    protected override void OnPointerPressed(PointerPressedEventArgs p_Args) => WindowHelper.Drag(this, p_Args);

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public void OnMinimizeButton(object? p_Sender, RoutedEventArgs _) => WindowHelper.MinimizeButton_Click(this);

    public void OnCloseButton(object? p_Sender, RoutedEventArgs _) => WindowHelper.CloseButton_Click(this);


    private void OnSettingsButton(object? p_Sender, RoutedEventArgs _)
    {
        var l_SettingsWindow = new SettingsWindow { DataContext = new SettingsWindowViewModel() };
        l_SettingsWindow.ShowDialog(this);
    }

    private void OnHistoryButton(object? p_Sender, RoutedEventArgs _)
    {
        var l_HistoryWindow = new HistoryWindow { DataContext = new HistoryWindowViewModel() };
        l_HistoryWindow.ShowDialog(this);
    }

    /// <summary>
    ///     Event handler for the resize grip.
    ///     Changes the GlobalScore of the application.
    ///     Makes sure that the scale is within the bounds.
    /// </summary>
    /// <param name="_"></param>
    /// <param name="p_E"></param>
    private void ResizeGrip_OnDragDelta(object? _, VectorEventArgs p_E)
    {
        if (WindowState != WindowState.Normal)
            return;

        var l_Config = AppConfiguration.StaticCache;

        /// Ensure that the steps are not too big (to avoid UI Flickering).
        var l_NewScale = l_Config.GlobalScale + Math.Sign(p_E.Vector.X) * (Math.Min(Math.Abs(p_E.Vector.X), 1) / 100);
        /// Ensure that the scale is within the bounds.
        l_NewScale = Math.Max(DefaultConfiguration.s_MinimumGlobalScale, Math.Min(DefaultConfiguration.s_MaximumGlobalScale, l_NewScale));

        if (Math.Abs(l_Config.GlobalScale - l_NewScale) < 0.01)
            return;

        /// Update the scale.
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            l_Config.GlobalScale = (float)l_NewScale;
        });
    }
}
