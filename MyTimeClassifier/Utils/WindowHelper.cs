using Avalonia.Controls;
using Avalonia.Input;

namespace MyTimeClassifier.Utils;

public static class WindowHelper
{
    public static void Drag(Window p_Sender, PointerPressedEventArgs p_Args)
    {
        if (!p_Args.GetCurrentPoint(p_Sender).Properties.IsLeftButtonPressed) return;

        p_Sender.BeginMoveDrag(p_Args);
    }

    public static void CloseButton_Click(Window p_Window) => p_Window.Close();

    public static void MinimizeButton_Click(Window p_Window) => p_Window.Hide();
}
