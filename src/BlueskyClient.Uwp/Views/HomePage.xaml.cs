using BlueskyClient.Collections;
using BlueskyClient.ViewModels;
using CommunityToolkit.WinUI;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

#nullable enable

namespace BlueskyClient.Views;

public sealed partial class HomePage : Page
{
    public HomePage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<HomePageViewModel>();
        FeedCollection = new HomeFeedCollection(ViewModel);

        Window.Current.SetTitleBar(TitleBar);
    }

    public HomePageViewModel ViewModel { get; }

    public HomeFeedCollection FeedCollection { get; }

    private string RefreshTooltip => $"{Strings.Resources.RefreshText} (Ctrl+R)";

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        try
        {
            await ViewModel.InitializeAsync(default);
        }
        catch (OperationCanceledException)
        {

        }

        // We set this up after the initialization because
        // setting it up only works after the feed is visible with items.
        // If we run it before the initialization is complete,
        // the scrollviewer can't be found.
        await Task.Delay(1); // Also required so that the listview's scrollviewer is discoverable.
        SetupRenderOutsideBounds(FeedListView);
    }

    private async void OnFeedSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.FirstOrDefault() is FeedGeneratorViewModel vm)
        {
            await ViewModel.ChangeFeedsCommand.ExecuteAsync(vm);
        }
    }

    private static void SetupRenderOutsideBounds(ListView? rawListView)
    {
        if (rawListView is ListView listView &&
            listView.FindDescendant<ScrollViewer>() is ScrollViewer s)
        {
            s.CanContentRenderOutsideBounds = true;
        }
    }
}
