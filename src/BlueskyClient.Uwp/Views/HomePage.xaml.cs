using BlueskyClient.Collections;
using BlueskyClient.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
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
        FeedCollection = new PaginatedCollection<FeedItemViewModel>(ViewModel, ViewModel.CollectionSource);
    }

    public HomePageViewModel ViewModel { get; }

    public PaginatedCollection<FeedItemViewModel> FeedCollection { get; }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        try
        {
            await ViewModel.InitializeAsync(default);
        }
        catch (OperationCanceledException)
        {

        }
    }

    private async void OnFeedSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.FirstOrDefault() is FeedGeneratorViewModel vm)
        {
            await ViewModel.ChangeFeedsCommand.ExecuteAsync(vm);
        }
    }
}
