using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using DynamicData;
using MyTimeClassifier.Database;
using MyTimeClassifier.UI.Models;
using ReactiveUI;

namespace MyTimeClassifier.UI.ViewModels;

/// <summary>
/// This class will be in charge of updating the Configuration singleton and updating the UI accordingly
/// </summary>
public class HistoryWindowViewModel : ViewModelBase, IDisposable
{
    /// <summary>
    /// Value to change if the user want to change the amount of items per page
    /// </summary>
    private const int ItemsPerPage = 6;

    /// <summary>
    /// Value should never change as it's just how it is on the UI, no matter what
    /// </summary>
    private const int ItemHeight = 52;

    private readonly int _maxPage;
    private int _currentPage;

    public HistoryWindowViewModel()
    {
        _maxPage = StaticRepo.GetTotalTaskCount() / ItemsPerPage - 1;
        ReLoadData();
    }

    public int ListBoxHeight => Math.Min(ItemsPerPage, TaskModels.Count) * ItemHeight;

    /// <summary>
    /// When the scroll viewer is set, we subscribe to the scroll changed event
    /// </summary>
    public ScrollViewer? ScrollViewer
    {
        get;
        set
        {
            if (field is not null)
                field.ScrollChanged -= OnScrollChanged;

            this.RaiseAndSetIfChanged(ref field, value);

            if (field is null)
                return;

            /* Subscribe to the scroll changed event */
            field.ScrollChanged += OnScrollChanged;
        }
    }

    public ObservableCollection<TaskModel> TaskModels { get; set => this.RaiseAndSetIfChanged(ref field, value); } = [];

    public void Dispose()
    {
        if (ScrollViewer is not null)
            ScrollViewer.ScrollChanged -= OnScrollChanged;

        ScrollViewer = null;
    }

    public void ReLoadData()
    {
        var prevCount = TaskModels.Count;
        /* Ensure the memory is cleared, and we don't create a new instance of the ObservableCollection */
        TaskModels.Clear();

        /* Add the new items (containing their own Jobs too) */
        TaskModels.AddRange(StaticRepo.GetTaskModels(Math.Max(_currentPage, 0), ItemsPerPage,
            addTake: ItemsPerPage));

        /* Ensure the Height of the list box is updated */
        if (prevCount != TaskModels.Count)
            this.RaisePropertyChanged(nameof(ListBoxHeight));

        /* Ensure the current page is valid */
        if (TaskModels.Count == 0 && _currentPage > 0)
        {
            _currentPage--;
            ReLoadData();
        }
    }

    private void OnScrollChanged(object? sender, ScrollChangedEventArgs @event)
    {
        /* Check if it's an offset delta change */
        if (@event.OffsetDelta == default)
            return;

        /* We only care about the scroll viewer */
        if (sender is not ScrollViewer { IsVisible: true } scrollViewer)
            return;

        var scrollBarPercentageDown = scrollViewer.Offset.Y / scrollViewer.Extent.Height;
        var scrollBarPercentageUp =
            (scrollViewer.Offset.Y + scrollViewer.Viewport.Height) / scrollViewer.Extent.Height;

        var currentPage = _currentPage;

        if (scrollBarPercentageDown == 0 && _currentPage > 0)
        {
            _currentPage--;

            // scroll to the previous 3 snap points
            ScrollViewer!.Offset = new Vector(0, 0.10f);
        }
        else if (Math.Abs(scrollBarPercentageUp - 1) < 0.01f && _currentPage < _maxPage)
        {
            _currentPage++;
            ScrollViewer!.Offset = new Vector(0, 0.10f);
        }

        if (currentPage != _currentPage)
            /* Reload the data */
            ReLoadData();
    }
}