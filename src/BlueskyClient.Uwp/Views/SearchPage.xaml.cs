﻿using BlueskyClient.Collections;
using BlueskyClient.Extensions.Uwp;
using BlueskyClient.ViewModels;
using JeniusApps.Common.Telemetry;
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
        FeedCollection = new PaginatedCollection<FeedItemViewModel>(ViewModel, ViewModel.CollectionSource);
        ActorCollection = new PaginatedCollection<AuthorViewModel>(ViewModel, ViewModel.ActorsCollectionSource);

        Window.Current.SetTitleBar(TitleBar);
    }

    public SearchPageViewModel ViewModel { get; }

    public PaginatedCollection<FeedItemViewModel> FeedCollection { get; }

    public PaginatedCollection<AuthorViewModel> ActorCollection { get; }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        App.Services.GetRequiredService<ITelemetry>().TrackPageView(nameof(SearchPage));

        try
        {
            await ViewModel.InitializeAsync(default);
        }
        catch (OperationCanceledException) { }

        SearchResultsListView.SetupRenderOutsideBounds();
        ActorResultsListView.SetupRenderOutsideBounds();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        ViewModel.Uninitialize();
    }

    private async void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (!ViewModel.NewSearchCommand.IsRunning)
        {
            await ViewModel.NewSearchCommand.ExecuteAsync(null);
        }
    }
}