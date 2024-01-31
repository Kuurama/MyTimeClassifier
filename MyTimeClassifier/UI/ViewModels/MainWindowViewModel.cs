using MyTimeClassifier.UI.Views;
using ReactiveUI;
using System;
using System.Windows.Input;

namespace MyTimeClassifier.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ICommand StatsCommand   { get; } = ReactiveCommand.Create(() => Console.WriteLine("StatsCommand invoked"));
    public ICommand HistoryCommand { get; } = ReactiveCommand.Create(() => Console.WriteLine("HistoryCommand invoked"));
    public ICommand SettingsCommand { get; } = ReactiveCommand.Create(() =>
    {
        var l_SettingsWindow = new SettingsWindow
        {
            DataContext = new SettingsWindowViewModel()
        };
        l_SettingsWindow.Show();
    });
}
