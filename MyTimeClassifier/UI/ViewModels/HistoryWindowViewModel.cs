using Avalonia;
using Avalonia.Controls;
using DynamicData;
using MyTimeClassifier.Database;
using MyTimeClassifier.UI.Models;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;

namespace MyTimeClassifier.UI.ViewModels;

/// <summary>
///     This class will be in charge of updating the Configuration singleton and updating the UI accordingly
/// </summary>
public class HistoryWindowViewModel : ViewModelBase
{
    /// <summary>
    ///     Value to change if the user want to change the amount of items per page
    /// </summary>
    private const int ITEMS_PER_PAGE = 6;
    /// <summary>
    ///     Value should never change as it's just how it is on the UI, no matter what
    /// </summary>
    private const int ITEM_HEIGHT = 52;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    private readonly int                             m_MaxPage;
    private          int                             m_CurrentPage;
    private          ScrollViewer?                   m_ScrollViewer;
    private          ObservableCollection<TaskModel> m_TaskModelModels = [];

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    public HistoryWindowViewModel()
    {
        m_MaxPage = StaticRepo.GetTotalTaskCount() / ITEMS_PER_PAGE - 1;
        ReLoadData();
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public int ListBoxItemSize { get => ITEM_HEIGHT; }
    public int ListBoxHeight   { get => Math.Min(ITEMS_PER_PAGE, TaskModels.Count) * ITEM_HEIGHT; }

    /// <summary>
    ///     When the scroll viewer is set, we subscribe to the scroll changed event
    /// </summary>
    public ScrollViewer? ScrollViewer
    {
        get => m_ScrollViewer;
        set
        {
            if (m_ScrollViewer is not null)
                m_ScrollViewer.ScrollChanged -= OnScrollChanged;

            this.RaiseAndSetIfChanged(ref m_ScrollViewer, value);

            if (m_ScrollViewer is null)
                return;

            /* Subscribe to the scroll changed event */
            m_ScrollViewer.ScrollChanged += OnScrollChanged;
        }
    }

    public ObservableCollection<TaskModel> TaskModels
    {
        get => m_TaskModelModels;
        set => this.RaiseAndSetIfChanged(ref m_TaskModelModels, value);
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public void ReLoadData()
    {
        var l_PrevCount = TaskModels.Count;
        /* Ensure the memory is cleared, and we don't create a new instance of the ObservableCollection */
        TaskModels.Clear();

        /* Add the new items (containing their own Jobs too) */
        TaskModels.AddRange(StaticRepo.GetTaskModels(Math.Max(m_CurrentPage, 0), ITEMS_PER_PAGE, p_AddTake: ITEMS_PER_PAGE));

        /* Ensure the Height of the list box is updated */
        if (l_PrevCount != TaskModels.Count)
            this.RaisePropertyChanged(nameof(ListBoxHeight));

        /* Ensure the current page is valid */
        if (TaskModels.Count == 0 && m_CurrentPage > 0)
        {
            m_CurrentPage--;
            ReLoadData();
        }
    }

    private void OnScrollChanged(object? p_Sender, ScrollChangedEventArgs p_Event)
    {
        /* Check if it's an offset delta change */
        if (p_Event.OffsetDelta == default(Vector))
            return;

        /* We only care about the scroll viewer */
        if (p_Sender is not ScrollViewer { IsVisible: true } l_ScrollViewer)
            return;

        var l_ScrollBarPercentageDown = l_ScrollViewer.Offset.Y                                    / l_ScrollViewer.Extent.Height;
        var l_ScrollBarPercentageUp   = (l_ScrollViewer.Offset.Y + l_ScrollViewer.Viewport.Height) / l_ScrollViewer.Extent.Height;

        var l_CurrentPage = m_CurrentPage;

        if (l_ScrollBarPercentageDown == 0 && m_CurrentPage > 0)
        {
            m_CurrentPage--;

            // scroll to the previous 3 snap points
            m_ScrollViewer!.Offset = new Vector(0, 0.10f);
        }
        else if (Math.Abs(l_ScrollBarPercentageUp - 1) < 0.01f && m_CurrentPage < m_MaxPage)
        {
            m_CurrentPage++;
            m_ScrollViewer!.Offset = new Vector(0, 0.10f);
        }

        if (l_CurrentPage != m_CurrentPage)
        {
            /* Reload the data */
            ReLoadData();
        }
    }
}
