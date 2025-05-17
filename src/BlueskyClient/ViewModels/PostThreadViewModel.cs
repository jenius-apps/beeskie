using Bluesky.NET.Models;
using BlueskyClient.Models;
using BlueskyClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.ViewModels;

public partial class PostThreadViewModel : ObservableObject
{
    private readonly IPostThreadService _postThreadService;
    private readonly IFeedItemViewModelFactory _feedItemViewModelFactory;

    public PostThreadViewModel(
        IPostThreadService postThreadService,
        IFeedItemViewModelFactory feedItemViewModelFactory)
    {
        _postThreadService = postThreadService;
        _feedItemViewModelFactory = feedItemViewModelFactory;
    }

    /// <summary>
    /// The viewmodel of the parent post.
    /// </summary>
    [ObservableProperty]
    private FeedItemViewModel? _mainPostViewModel;

    /// <summary>
    /// Top-level replies to the parent post.
    /// </summary>
    public ObservableCollection<FeedItemViewModel> Replies { get; } = [];

    public async Task InitializeAsync(PostThreadArgs? args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (args?.AtUri is not { Length: > 0 } atUri)
        {
            // TODO placeholder UI.
            return;
        }

        PostThread? thread = await _postThreadService.GetThreadAsync(atUri, cancellationToken);
        if (thread?.Post is null)
        {
            // TOOD placeholder UI.
            return;
        }

        MainPostViewModel = _feedItemViewModelFactory.CreateViewModel(thread.Post, isPostThreadParent: true);

        foreach (PostThread reply in thread.Replies.OrderBy(x => x.Post?.Record?.CreatedAtUtc))
        {
            if (reply.Post is null)
            {
                continue;
            }

            Replies.Add(_feedItemViewModelFactory.CreateViewModel(reply.Post));
        }
    }
}
