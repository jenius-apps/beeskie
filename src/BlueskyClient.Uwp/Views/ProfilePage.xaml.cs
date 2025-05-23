﻿using JeniusApps.Common.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

#nullable enable

namespace BlueskyClient.Views;

/// <summary>
/// Page used to load the user's personal profile.
/// </summary>
public sealed partial class ProfilePage : Page
{
    private readonly CancellationTokenSource _cts = new();

    public ProfilePage()
    {
        this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        App.Services.GetRequiredService<ITelemetry>().TrackPageView(nameof(ProfilePage));

        try
        {
            // Passing null is on purpose, since this would trigger loading the user's personal profile.
            await ProfileControl.ViewModel.InitializeAsync(null, _cts.Token);
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
