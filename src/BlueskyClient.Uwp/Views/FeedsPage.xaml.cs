using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BlueskyClient.ViewModels;
using JeniusApps.Common.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

#nullable enable

namespace BlueskyClient.Views;
public sealed partial class FeedsPage : Page
{
    public FeedsPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<FeedsPageViewModel>();
    }

    public FeedsPageViewModel ViewModel { get; }

    protected async override void OnNavigatedTo(NavigationEventArgs e)
    {
        App.Services.GetRequiredService<ITelemetry>().TrackPageView(nameof(FeedsPage));


        // TODO cancel on nav from
        await ViewModel.InitializeAsync(default);
    }
}
