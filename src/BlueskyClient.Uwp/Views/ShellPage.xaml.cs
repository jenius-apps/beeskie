using BlueskyClient.Constants;
using BlueskyClient.Models;
using BlueskyClient.ViewModels;
using JeniusApps.Common.Telemetry;
using JeniusApps.Common.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

#nullable enable

namespace BlueskyClient.Views;

public sealed partial class ShellPage : Page
{
    public ShellPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<ShellPageViewModel>();
    }

    public ShellPageViewModel ViewModel { get; }

    public string DisplayTitle => $"Beeskie {SystemInformation.Instance.ApplicationVersion.ToFormattedString().TrimEnd('0').TrimEnd('.')}";

    protected async override void OnNavigatedTo(NavigationEventArgs e)
    {
        App.Services.GetRequiredService<ITelemetry>().TrackPageView(nameof(ShellPage));
        App.Services.GetRequiredKeyedService<INavigator>(NavigationConstants.ContentNavigatorKey).SetFrame(ContentFrame);
        await ViewModel.InitializeAsync(e.Parameter as ShellPageNavigationArgs ?? new()).ConfigureAwait(false);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        ViewModel.Unitialize();
    }

    private void OnImagePreviewBackgroundKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key is VirtualKey.Escape)
        {
            e.Handled = true;
            ViewModel.CloseImageViewerCommand.Execute("escapeKey");
        }
    }

    private void OnImagePreviewBackgroundClicked(object sender, TappedRoutedEventArgs e)
    {
        if (e.OriginalSource is Grid g && g == SmokeGrid)
        {
            e.Handled = true;
            ViewModel.CloseImageViewerCommand.Execute("backgroundClicked");
        }
    }

    private void OnFeedbackClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        App.Services.GetRequiredService<ITelemetry>().TrackEvent(TelemetryConstants.FeedbackClicked);
    }

    private void OnProfileControlClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is Button b)
        {
            FlyoutBase.ShowAttachedFlyout(b);
        }
    }
}
