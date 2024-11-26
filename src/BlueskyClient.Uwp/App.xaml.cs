using BlueskyClient.Constants;
using BlueskyClient.Views;
using JeniusApps.Common.Settings;
using JeniusApps.Common.Telemetry;
using JeniusApps.Common.Tools;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources.Core;
using Windows.Storage.AccessCache;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace BlueskyClient;

sealed partial class App : Application
{
    private static Frame? AppFrame;

    public static bool IsRightToLeftLanguage
    {
        get
        {
            string flowDirectionSetting = ResourceContext.GetForCurrentView().QualifierValues["LayoutDirection"];
            return flowDirectionSetting == "RTL";
        }
    }

    public App()
    {
        this.InitializeComponent();
        this.Suspending += OnSuspending;
        this.UnhandledException += OnUnhandledException;
    }

    private async void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        if (_serviceProvider is { } serviceProvider)
        {
            var telemetry = serviceProvider.GetRequiredService<ITelemetry>();
            telemetry.TrackError(e.Exception);
            await telemetry.FlushAsync();
        }
    }

    private async void OnSuspending(object sender, SuspendingEventArgs e)
    {
        var d = e.SuspendingOperation.GetDeferral();
        StorageApplicationPermissions.FutureAccessList.Clear();

        if (_serviceProvider is { } serviceProvider)
        {
            await serviceProvider.GetRequiredService<ITelemetry>().FlushAsync();
        }
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        await ActivateAsync(args);
    }

    private Task ActivateAsync(LaunchActivatedEventArgs args)
    {
        _serviceProvider = ConfigureServices();

        if (Window.Current.Content is not Frame rootFrame)
        {
            rootFrame = new();
            Window.Current.Content = rootFrame;
        }

        AppFrame = rootFrame;
        var navigator = Services.GetRequiredKeyedService<INavigator>(NavigationConstants.RootNavigatorKey);
        navigator.SetFrame(rootFrame);

        if (args.PrelaunchActivated is false)
        {
            CoreApplication.EnablePrelaunch(true);

            if (rootFrame.Content is null)
            {
                var storedHandle = Services.GetRequiredService<IUserSettings>().Get<string>(UserSettingsConstants.SignedInDIDKey);

                if (string.IsNullOrEmpty(storedHandle))
                {
                    rootFrame.Navigate(typeof(SignInPage));
                }
                else
                {
                    rootFrame.Navigate(typeof(ShellPage), storedHandle);
                }
            }

            Window.Current.Activate();
            ConfigureUI();
            Services.GetRequiredService<ITelemetry>().TrackEvent(TelemetryConstants.Launched);
        }

        return Task.CompletedTask;
    }

    private void ConfigureUI()
    {
        if (AppFrame is { } rootFrame)
        {
            if (IsRightToLeftLanguage)
            {
                rootFrame.FlowDirection = FlowDirection.RightToLeft;
            }

            CustomizeTitleBar(darkTheme: rootFrame.ActualTheme is ElementTheme.Dark);
        }
    }

    private void CustomizeTitleBar(bool darkTheme)
    {
        CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

        var viewTitleBar = ApplicationView.GetForCurrentView().TitleBar;
        viewTitleBar.ButtonBackgroundColor = Colors.Transparent;
        viewTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        viewTitleBar.ButtonForegroundColor = darkTheme ? Colors.LightGray : Colors.Black;
    }
}
