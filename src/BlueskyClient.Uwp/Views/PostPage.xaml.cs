using BlueskyClient.Constants;
using BlueskyClient.Models;
using JeniusApps.Common.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
        var args = e.Parameter as PostThreadArgs;
        var telemetry = App.Services.GetRequiredService<ITelemetry>();
        telemetry.TrackPageView(nameof(PostPage));
        telemetry.TrackEvent(TelemetryConstants.PostThreadPageOpened, new Dictionary<string, string>
        {
            { "source", args?.NavigationRequester ?? "null" }
        });

        try
        {
            await PostThread.ViewModel.InitializeAsync(args, _cts.Token);
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
