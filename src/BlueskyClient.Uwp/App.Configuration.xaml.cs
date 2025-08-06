using Bluesky.NET.ApiClients;
using Bluesky.NET.Models;
using BlueskyClient.Caches;
using BlueskyClient.Constants;
using BlueskyClient.Services;
using BlueskyClient.Services.Uwp;
using BlueskyClient.Tools;
using BlueskyClient.Tools.Uwp;
using BlueskyClient.ViewModels;
using BlueskyClient.Views;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Extensions.DependencyInjection;
using JeniusApps.Common.Settings;
using JeniusApps.Common.Settings.Uwp;
using JeniusApps.Common.Telemetry;
using JeniusApps.Common.Tools;
using JeniusApps.Common.Tools.Uwp;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.System.Profile;

#nullable enable

namespace BlueskyClient;

partial class App
{
    private IServiceProvider? _serviceProvider;

    public static IServiceProvider Services
    {
        get
        {
            IServiceProvider? serviceProvider = ((App)Current)._serviceProvider;

            if (serviceProvider is null)
            {
                ThrowHelper.ThrowInvalidOperationException("The service provider is not initialized");
            }

            return serviceProvider;
        }
    }


    /// <summary>
    /// Builds the <see cref="IServiceProvider"/> instance with the required services.
    /// </summary>
    /// <param name="appsettings">The <see cref="IAppSettings"/> instance to use, if available.</param>
    /// <returns>The resulting service provider.</returns>
    private static IServiceProvider ConfigureServices()
    {
        ServiceCollection collection = new();
        ConfigureServices(collection);

        collection.AddSingleton<ITelemetry, AppInsightsTelemetry>(s =>
        {
            string apiKey = s.GetRequiredService<IAppSettings>().TelemetryApiKey;
            var context = GetContext();
            return new AppInsightsTelemetry(apiKey, context: context);
        });

        collection.AddKeyedSingleton<INavigator, Navigator>(NavigationConstants.RootNavigatorKey, (serviceProvider, key) =>
        {
            return new Navigator(new Dictionary<string, Type>
            {
                { NavigationConstants.ShellPage, typeof(ShellPage) },
                { NavigationConstants.SignInPage, typeof(SignInPage) },
            });
        });

        collection.AddKeyedSingleton<INavigator, Navigator>(NavigationConstants.ContentNavigatorKey, (serviceProvider, key) =>
        {
            return new Navigator(new Dictionary<string, Type>
            {
                { NavigationConstants.HomePage, typeof(HomePage) },
                { NavigationConstants.NotificationsPage, typeof(NotificationsPage) },
                { NavigationConstants.ProfilePage, typeof(ProfilePage) },
                { NavigationConstants.AuthorPage, typeof(AuthorPage) },
                { NavigationConstants.FeedsPage, typeof(FeedsPage) },
                { NavigationConstants.SearchPage, typeof(SearchPage) },
                { NavigationConstants.PostPage, typeof(PostPage) },
            });
        });

        collection.AddTransient((serviceProvider) =>
        {
            return new SignInPageViewModel(
                serviceProvider.GetRequiredService<IAuthenticationService>(),
                serviceProvider.GetRequiredKeyedService<INavigator>(NavigationConstants.RootNavigatorKey),
                serviceProvider.GetRequiredService<IUserSettings>(),
                serviceProvider.GetRequiredService<ITelemetry>());
        });

        collection.AddTransient((serviceProvider) =>
        {
            return new ShellPageViewModel(
                serviceProvider.GetRequiredService<ILocalizer>(),
                serviceProvider.GetRequiredService<ITelemetry>(),
                serviceProvider.GetRequiredKeyedService<INavigator>(NavigationConstants.ContentNavigatorKey),
                serviceProvider.GetRequiredKeyedService<INavigator>(NavigationConstants.RootNavigatorKey),
                serviceProvider.GetRequiredService<IProfileService>(),
                serviceProvider.GetRequiredService<IDialogService>(),
                serviceProvider.GetRequiredService<IAuthenticationService>(),
                serviceProvider.GetRequiredService<IImageViewerService>(),
                serviceProvider.GetRequiredService<IAuthorViewModelFactory>(),
                serviceProvider.GetRequiredService<INotificationsService>(),
                serviceProvider.GetRequiredService<ISearchInPlaceRequester>(),
                serviceProvider.GetRequiredService<IRefreshPageRequester>());
        });

        collection.AddSingleton<ISecureCredentialStorage>(_ => new WindowsCredentialStorage("blueskyClientCredentials"));
        collection.AddSingleton<IUserSettings>(_ => new LocalSettings(UserSettingsConstants.Defaults));

        IServiceProvider provider = collection.BuildServiceProvider();
        return provider;
    }

    /// <summary>
    /// Configures services used by the app.
    /// </summary>
    /// <param name="services">The target <see cref="IServiceCollection"/> instance to register services with.</param>
    [Singleton(typeof(BlueskyApiClient), typeof(IBlueskyApiClient))]
    [Singleton(typeof(AuthenticationService), typeof(IAuthenticationService))]
    [Singleton(typeof(TimelineService), typeof(ITimelineService))]
    [Singleton(typeof(SearchService), typeof(ISearchService))]
    [Singleton(typeof(DiscoverService), typeof(IDiscoverService))]
    [Singleton(typeof(FeedItemViewModelFactory), typeof(IFeedItemViewModelFactory))]
    [Singleton(typeof(NotificationViewModelFactory), typeof(INotificationViewModelFactory))]
    [Singleton(typeof(FeedGeneratorViewModelFactory), typeof(IFeedGeneratorViewModelFactory))]
    [Singleton(typeof(AuthorViewModelFactory), typeof(IAuthorViewModelFactory))]
    [Singleton(typeof(NotificationsService), typeof(INotificationsService))]
    [Singleton(typeof(ProfileCache), typeof(Caches.ICache<Author>))]
    [Singleton(typeof(FeedGeneratorCache), typeof(Caches.ICache<FeedGenerator>))]
    [Singleton(typeof(ProfileService), typeof(IProfileService))]
    [Singleton(typeof(FeedGeneratorService), typeof(IFeedGeneratorService))]
    [Singleton(typeof(PostSubmissionService), typeof(IPostSubmissionService))]
    [Singleton(typeof(DialogService), typeof(IDialogService))]
    [Singleton(typeof(AppSettings), typeof(IAppSettings))]
    [Singleton(typeof(ImageViewerService), typeof(IImageViewerService))]
    [Singleton(typeof(ReswLocalizer), typeof(ILocalizer))]
    [Singleton(typeof(FileReaderWriter), typeof(IFileReadWriter))]
    [Singleton(typeof(FutureAccessFilePicker), typeof(IFutureAccessFilePicker))]
    [Singleton(typeof(UploadBlobService), typeof(IUploadBlobService))]
    [Singleton(typeof(FacetService), typeof(IFacetService))]
    [Singleton(typeof(SearchInPlaceRequester), typeof(ISearchInPlaceRequester))]
    [Singleton(typeof(RefreshPageRequester), typeof(IRefreshPageRequester))]
    [Singleton(typeof(ModerationService), typeof(IModerationService))]
    [Singleton(typeof(PostThreadService), typeof(IPostThreadService))]
    [Transient(typeof(HomePageViewModel))]
    [Transient(typeof(NotificationsPageViewModel))]
    [Transient(typeof(ProfileControlViewModel))]
    [Transient(typeof(NewPostViewModel))]
    [Transient(typeof(FeedsPageViewModel))]
    [Transient(typeof(SearchPageViewModel))]
    [Transient(typeof(PostThreadViewModel))]
    private static partial void ConfigureServices(IServiceCollection services);

    private static TelemetryContext? GetContext()
    {
        var context = new TelemetryContext();
        context.Session.Id = Guid.NewGuid().ToString();
        context.Component.Version = SystemInformation.Instance.ApplicationVersion.ToFormattedString();
        context.GlobalProperties.Add("isFirstRun", SystemInformation.Instance.IsFirstRun.ToString());

        if (ApplicationData.Current.LocalSettings.Values[UserSettingsConstants.LocalUserIdKey] is string { Length: > 0 } id)
        {
            context.User.Id = id;
        }
        else
        {
            string userId = Guid.NewGuid().ToString();
            ApplicationData.Current.LocalSettings.Values[UserSettingsConstants.LocalUserIdKey] = userId;
            context.User.Id = userId;
        }

        // Ref: https://learn.microsoft.com/en-us/answers/questions/1563897/uwp-and-winui-how-to-check-my-os-version-through-c
        ulong version = ulong.Parse(AnalyticsInfo.VersionInfo.DeviceFamilyVersion);
        ulong major = (version & 0xFFFF000000000000L) >> 48;
        ulong minor = (version & 0x0000FFFF00000000L) >> 32;
        ulong build = (version & 0x00000000FFFF0000L) >> 16;
        context.Device.OperatingSystem = $"Windows {major}.{minor}.{build}";

        return context;
    }
}
