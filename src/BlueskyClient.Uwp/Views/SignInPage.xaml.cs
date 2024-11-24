using BlueskyClient.Constants;
using BlueskyClient.ViewModels;
using JeniusApps.Common.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

#nullable enable

namespace BlueskyClient.Views;

public sealed partial class SignInPage : Page
{
    public SignInPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<SignInPageViewModel>();
    }

    public SignInPageViewModel ViewModel { get; }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        App.Services.GetRequiredService<ITelemetry>().TrackPageView(nameof(SignInPage));
    }

    private void OnAppPassHelpClicked(object sender, RoutedEventArgs e)
    {
        App.Services.GetRequiredService<ITelemetry>().TrackEvent(TelemetryConstants.AppPasswordHelpClicked);
    }

    private async void OnPasswordBoxKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key is VirtualKey.Enter && 
            ViewModel.AppPasswordInput.Length > 0 &&
            ViewModel.UserHandleInput.Length > 0)
        {
            e.Handled = true;
            await ViewModel.SignInCommand.ExecuteAsync(null);
        }
    }
}
