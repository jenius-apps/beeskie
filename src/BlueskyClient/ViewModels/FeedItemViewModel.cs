using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using BlueskyClient.Constants;
using BlueskyClient.Extensions;
using BlueskyClient.Models;
using BlueskyClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Humanizer;
using Humanizer.Localisation;
using JeniusApps.Common.Tools;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.ViewModels;

public partial class FeedItemViewModel : ObservableObject
{
    private readonly IPostSubmissionService _postSubmissionService;
    private readonly IDialogService _dialogService;
    private readonly ILocalizer _localizer;
    private readonly FeedPostReason? _reason;
    private readonly INavigator _contentNavigator;

    private string? _likeUri;
    private string? _repostUri;

    public FeedItemViewModel(
        FeedPost post,
        FeedPostReason? reason,
        IPostSubmissionService postSubmissionService,
        IDialogService dialogService,
        ILocalizer localizer,
        IAuthorViewModelFactory authorFactory,
        INavigator contentNavigator)
    {
        Post = post;
        _reason = reason;
        AuthorViewModel = authorFactory.Create(post.Author);
        _postSubmissionService = postSubmissionService;
        _dialogService = dialogService;
        _localizer = localizer;
        _contentNavigator = contentNavigator;

        IsLiked = post.Viewer?.Like is not null;
        IsReposted = post.Viewer?.Repost is not null;
        ReplyCount = post.GetReplyCount();
        RepostCount = post.GetRepostCount();
        LikeCount = post.GetLikeCount();

        _likeUri = post.Viewer?.Like;
        _repostUri = post.Viewer?.Repost;
    }

    public AuthorViewModel AuthorViewModel { get; }

    public FeedPost Post { get; }

    public string TimeSinceCreation
    {
        get
        {
            var now = DateTime.Now;

            if (Post.Record?.CreatedAtUtc?.ToLocalTime() is not DateTime createdAt ||
                createdAt > now)
            {
                return string.Empty;
            }

            return now.Subtract(createdAt).Humanize(maxUnit: TimeUnit.Year);
        }
    }

    public bool IsRepost => _reason?.Type.EndsWith("#reasonRepost", StringComparison.OrdinalIgnoreCase) ?? false;

    public string ReposterName => IsRepost
        ? _reason?.By?.DisplayName ?? string.Empty
        : string.Empty;

    public string RepostCaption => IsRepost
        ? _localizer.GetString("RepostCaption", ReposterName)
        : string.Empty;

    public PostEmbed? PostEmbed => Post?.Embed;

    public FeedRecord? QuotedPost =>
        (Post?.Embed?.Record?.Record ?? Post?.Embed?.Record) is FeedRecord record &&
        record.Type?.GetRecordType() is not RecordType.StarterPack
            ? record
            : null;

    [ObservableProperty]
    private bool _isLiked;

    [ObservableProperty]
    private bool _isReposted;

    [ObservableProperty]
    private string _replyCount = string.Empty;

    [ObservableProperty]
    private string _repostCount = string.Empty;

    [ObservableProperty]
    private string _likeCount = string.Empty;

    /// <summary>
    /// Navigates to the profile page of the user associated wtih this feed item.
    /// </summary>
    [RelayCommand]
    private void OpenProfile()
    {
        _contentNavigator.NavigateTo(NavigationConstants.AuthorPage, new ProfileNavigationArgs
        {
            Author = AuthorViewModel.Author
        });
    }

    [RelayCommand]
    private void OpenPostThread()
    {
        _contentNavigator.NavigateTo(NavigationConstants.PostPage);
    }

    [RelayCommand]
    private async Task ReplyAsync()
    {
        await _dialogService.OpenReplyDialogAsync(Post);
    }

    [RelayCommand]
    private async Task LikeClickedAsync(CancellationToken ct)
    {
        if (IsLiked)
        {
            var result = await _postSubmissionService.LikeOrRepostUndoAsync(RecordType.Like, _likeUri!, ct);

            if (result)
            {
                LikeCount = Math.Max(0, int.Parse(LikeCount) - 1).ToString();
            }
        }
        else
        {
            var result = await _postSubmissionService.LikeOrRepostAsync(RecordType.Like, Post.Uri, Post.Cid);

            if (result is not null)
            {
                _likeUri = result;
                LikeCount = (int.Parse(LikeCount) + 1).ToString();
            }
        }

        IsLiked = !IsLiked;
    }

    [RelayCommand]
    private async Task QuotePostAsync()
    {
        await _dialogService.OpenQuoteDialogAsync(Post);
    }

    [RelayCommand]
    private async Task RepostClickedAsync(CancellationToken ct)
    {
        if (IsReposted)
        {
            //TODO: Recover URL and add it to viewer repost
            var result = await _postSubmissionService.LikeOrRepostUndoAsync(RecordType.Repost, _repostUri!, ct);

            if (result)
            {
                RepostCount = Math.Max(0, int.Parse(RepostCount) - 1).ToString();
            }
        }
        else
        {
            var result = await _postSubmissionService.LikeOrRepostAsync(RecordType.Repost, Post.Uri, Post.Cid);

            if (result is not null)
            {
                _repostUri = result;
                RepostCount = int.Parse(RepostCount + 1).ToString();
            }
        }

        IsReposted = !IsReposted;
    }

    public override string ToString()
    {
        return $"{AuthorViewModel.DisplayName}: {Post.Record?.Text}";
    }
}
