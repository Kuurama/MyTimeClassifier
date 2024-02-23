using MyTimeClassifier.Database;
using MyTimeClassifier.Database.Entities;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace MyTimeClassifier.UI.ViewModels;

/// <summary>
///     This class will be in charge of updating the Configuration singleton and updating the UI accordingly
/// </summary>
public class HistoryWindowViewModel : ViewModelBase
{
    private ObservableCollection<Task> m_Tasks = null!;

    public HistoryWindowViewModel()
    {
        using var l_DBContext = new AppDbContext();
        Tasks = new ObservableCollection<Task>(l_DBContext.Tasks);
    }

    public ObservableCollection<Task> Tasks
    {
        get => m_Tasks;
        set => this.RaiseAndSetIfChanged(ref m_Tasks, value);
    }
}
