using BlueskyClient.ViewModels;
using CommunityToolkit.WinUI;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace BlueskyClient.Extensions.Uwp;

public static class ListViewExtensions
{
    /// <summary>
    /// Updates the given listview in-place to render outside bounds.
    /// </summary>
    /// <param name="rawListView">The listview to update.</param>
    /// <returns>True if it was successful, false otherwise.</returns>
    public static bool SetupRenderOutsideBounds(this ListView? rawListView)
    {
        if (rawListView is ListView listView &&
            listView.FindDescendant<ScrollViewer>() is ScrollViewer s)
        {
            s.CanContentRenderOutsideBounds = true;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Helper method for navigating to post page when a feed item is clicked.
    /// </summary>
    public static void OnFeedListViewItemClicked(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is FeedItemViewModel vm)
        {
            vm.OpenPostThreadCommand.Execute(null);
        }
    }
}
