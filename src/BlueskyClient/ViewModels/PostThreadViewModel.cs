using BlueskyClient.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.ViewModels;

public partial class PostThreadViewModel : ObservableObject
{
    public async Task InitializeAsync(PostThreadArgs? args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (args is null)
        {
            // TODO placeholder UI.
            return;
        }

        await Task.Delay(1);
    }
}
