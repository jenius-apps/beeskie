using BlueskyClient.Constants;
using BlueskyClient.ViewModels;
using JeniusApps.Common.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml.Controls;
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

    private void OnAppPassHelpClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        App.Services.GetRequiredService<ITelemetry>().TrackEvent(TelemetryConstants.AppPasswordHelpClicked);
    }
}
