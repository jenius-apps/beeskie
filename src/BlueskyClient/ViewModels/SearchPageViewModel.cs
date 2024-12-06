using BlueskyClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.ViewModels;

public partial class SearchPageViewModel : ObservableObject
{
    private readonly IAuthenticationService _authenticationService;

    public SearchPageViewModel(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task InitializeAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        await Task.Delay(1);
    }
}
