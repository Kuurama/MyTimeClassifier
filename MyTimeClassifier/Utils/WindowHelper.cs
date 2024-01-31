using Avalonia.Controls;
using Avalonia.Input;

namespace MyTimeClassifier.Utils;

public static class WindowHelper
{
    public static void Drag(Window p_Sender, PointerPressedEventArgs p_Args)
    {
        if (p_Args.GetCurrentPoint(p_Sender).Properties.IsLeftButtonPressed)
            p_Sender.BeginMoveDrag(p_Args);
    }

    public static void CloseButton_Click(Window p_Window) => p_Window.Close();

    public static void MinimizeButton_Click(Window p_Window) => p_Window.WindowState = WindowState.Minimized;


    /*public static void MinimizeButton_Click(object p_Sender, RoutedEventArgs p_RoutedEventArgs)
    {
        if (p_Sender is not Button l_Button)
            return;
        var l_Window = Window.GetWindow(l_Button);
        if (l_Window is null)
            return;

        l_Window.WindowState = WindowState.Minimized;
    }

    public static void MaximizeButton_Click(object p_Sender, RoutedEventArgs p_RoutedEventArgs)
    {
        if (p_Sender is not Button l_Button)
            return;
        var l_Window = Window.GetWindow(l_Button);
        if (l_Window is null)
            return;

        l_Window.WindowState = l_Window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    public static void CloseButton_Click(object p_Sender, RoutedEventArgs p_RoutedEventArgs)
    {
        if (p_Sender is not Button l_Button)
            return;
        var l_Window = Window.GetWindow(l_Button);

        l_Window?.Close();
    }

    public static void HideWindow_Click(object p_Sender, RoutedEventArgs p_RoutedEventArgs)
    {
        if (p_Sender is not Button l_Button)
            return;
        var l_Window = Window.GetWindow(l_Button);

        l_Window?.Hide();
    }

    public static void OnDrag(object p_Sender, MouseButtonEventArgs p_EventArgs)
    {
        if (p_Sender is not Window l_Window)
            return;

        l_Window.BeginMoveDrag(p_EventArgs);

        try {
            /// Simply add the logic later to only be able to move when grabing the border
            l_Window.DragMove();
        }
        catch {
            //Ignored
        }
    }

    /// <summary>
    ///     Terminate the program (Here on window closed event).
    /// </summary>
    /// <param name="p_Sender"></param>
    /// <param name="p_E"></param>
    public static void OnMainWindowClosed(object? p_Sender, CancelEventArgs p_CancelEventArgs)
    {
        App.SafeReleaseMutex();
        Environment.Exit(Environment.ExitCode); // Prevent memory leak
    }

    /// <summary>
    /// Close the current window + release the mutex + exit the application
    /// </summary>
    /// <param name="p_Sender"></param>
    /// <param name="p_RoutedEventArgs"></param>
    public static void CloseAppButton_Click(object p_Sender, RoutedEventArgs p_RoutedEventArgs)
    {
        if (p_Sender is not Button l_Button)
            return;
        var l_Window = Window.GetWindow(l_Button);

        l_Window?.Close();

        App.SafeReleaseMutex();
        Environment.Exit(Environment.ExitCode); // Prevent memory leak
    }*/
}
