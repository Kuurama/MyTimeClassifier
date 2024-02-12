using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using MyTimeClassifier.Configuration;
using MyTimeClassifier.UI.ViewModels;
using MyTimeClassifier.Utils;
using System;

namespace MyTimeClassifier.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        /* Register the UI related events into the DataContext */
        DataContext = new MainWindowViewModel(JobSelector);
        AvaloniaXamlLoader.Load(this);
        this.FindControl<Thumb>("ResizeGrip")?.AddHandler(Thumb.DragDeltaEvent, ResizeGrip_OnDragDelta);
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    protected override void OnPointerPressed(PointerPressedEventArgs p_Args) => WindowHelper.Drag(this, p_Args);

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public void OnMinimizeButton(object? p_Sender, RoutedEventArgs _) => WindowHelper.MinimizeButton_Click(this);
    public void OnCloseButton(object?    p_Sender, RoutedEventArgs _) => WindowHelper.CloseButton_Click(this);

    public void OnSettingsButton(object? p_Sender, RoutedEventArgs _)
    {
        var l_SettingsWindow = new SettingsWindow { DataContext = new SettingsWindowViewModel() };
        l_SettingsWindow.ShowDialog(this);
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
        l_NewScale = Math.Max(0.35f, Math.Min(2.0f, l_NewScale));

        if (Math.Abs(l_Config.GlobalScale - l_NewScale) < 0.01)
            return;

        /// Update the scale.
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            l_Config.GlobalScale = (float)l_NewScale;
        });
    }
}
