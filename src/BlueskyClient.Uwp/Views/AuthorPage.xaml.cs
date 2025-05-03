using BlueskyClient.Models;
using JeniusApps.Common.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

#nullable enable

namespace BlueskyClient.Views;

/// <summary>
/// Page used to display an author's feed. Not designed to be used
/// to view the user's own profile.
/// </summary>
public sealed partial class AuthorPage : Page
{
    private readonly CancellationTokenSource _cts = new();

    public AuthorPage()
    {
        this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        App.Services.GetRequiredService<ITelemetry>().TrackPageView(nameof(AuthorPage));

        try
        {
            await ProfileControl.ViewModel.InitializeAsync(e.Parameter as ProfileNavigationArgs, _cts.Token);
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
