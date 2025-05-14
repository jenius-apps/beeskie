using BlueskyClient.Models;
using JeniusApps.Common.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

#nullable enable

namespace BlueskyClient.Views;

public sealed partial class PostPage : Page
{
    private readonly CancellationTokenSource _cts = new();

    public PostPage()
    {
        this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        App.Services.GetRequiredService<ITelemetry>().TrackPageView(nameof(PostPage));

        try
        {
            await PostThread.ViewModel.InitializeAsync(e.Parameter as PostThreadArgs, _cts.Token);
        }
        catch (OperationCanceledException)
        {

        }
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        _cts.Cancel();
    }
}
