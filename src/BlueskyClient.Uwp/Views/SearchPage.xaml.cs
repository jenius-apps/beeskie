using BlueskyClient.Collections;
using BlueskyClient.Extensions.Uwp;
using BlueskyClient.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

#nullable enable

namespace BlueskyClient.Views;

public sealed partial class SearchPage : Page
{
    public SearchPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<SearchPageViewModel>();
        FeedCollection = new PaginatedCollection<FeedItemViewModel>(ViewModel);

        Window.Current.SetTitleBar(TitleBar);
    }

    public SearchPageViewModel ViewModel { get; }

    public PaginatedCollection<FeedItemViewModel> FeedCollection { get; }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        try
        {
            await ViewModel.InitializeAsync(default);
        }
        catch (OperationCanceledException) { }

        SearchResultsListView.SetupRenderOutsideBounds();
    }

    private async void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (!ViewModel.NewSearchCommand.IsRunning)
        {
            await ViewModel.NewSearchCommand.ExecuteAsync(null);
        }
    }
}
