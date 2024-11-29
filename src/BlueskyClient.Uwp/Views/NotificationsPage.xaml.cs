using BlueskyClient.ViewModels;
using JeniusApps.Common.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

#nullable enable

namespace BlueskyClient.Views;

public sealed partial class NotificationsPage : Page
{
    public NotificationsPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<NotificationsPageViewModel>();
    }
    
    public NotificationsPageViewModel ViewModel { get; }

    protected async override void OnNavigatedTo(NavigationEventArgs e)
    {
        App.Services.GetRequiredService<ITelemetry>().TrackPageView(nameof(NotificationsPage));
        await ViewModel.InitializeAsync();
    }
}
