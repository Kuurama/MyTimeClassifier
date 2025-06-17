using Avalonia.Controls;
using Avalonia.Input;

namespace MyTimeClassifier.Utils;

public static class WindowHelper
{
    public static void Drag(Window sender, PointerPressedEventArgs args)
    {
        if (!args.GetCurrentPoint(sender).Properties.IsLeftButtonPressed) return;

        sender.BeginMoveDrag(args);
    }

    public static void CloseButton_Click(Window window) => window.Close();

    public static void MinimizeButton_Click(Window window) => window.Hide();
}