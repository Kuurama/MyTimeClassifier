﻿using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MyTimeClassifier.Configuration;
using MyTimeClassifier.UI.ViewModels;
using MyTimeClassifier.Utils;

namespace MyTimeClassifier.UI.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
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
        /* Save the settings */
        AppConfiguration.SaveConfiguration();

        /* Close the window */
        WindowHelper.CloseButton_Click(this);
    }

    private void OnJobsEdit(object? p_Sender, RoutedEventArgs _)
    {
        var l_JobEditWindow = new JobEditWindow { DataContext = new JobEditWindowViewModel() };
        l_JobEditWindow.ShowDialog(this);
    }
}
