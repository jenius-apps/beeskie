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
}
