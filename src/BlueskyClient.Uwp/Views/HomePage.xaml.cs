using BlueskyClient.Collections;
using BlueskyClient.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
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
    }

    public HomePageViewModel ViewModel { get; }

    public HomeFeedCollection FeedCollection { get; }

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
}
