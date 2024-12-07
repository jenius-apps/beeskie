using BlueskyClient.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

public sealed partial class SearchPage : Page
{
    public SearchPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<SearchPageViewModel>();

        Window.Current.SetTitleBar(TitleBar);
    }

    public SearchPageViewModel ViewModel { get; }

    private async void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (!ViewModel.NewSearchCommand.IsRunning)
        {
            await ViewModel.NewSearchCommand.ExecuteAsync(null);
        }
    }
}
