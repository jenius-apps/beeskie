using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using BlueskyClient.Extensions;
using BlueskyClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Humanizer;
using Humanizer.Localisation;
using JeniusApps.Common.Tools;
using System;
using System.Threading.Tasks;

namespace BlueskyClient.ViewModels;

public partial class FeedItemViewModel : ObservableObject
{
    private readonly IPostSubmissionService _postSubmissionService;
    private readonly IDialogService _dialogService;
    private readonly ILocalizer _localizer;
    private readonly FeedPostReason? _reason;

    public FeedItemViewModel(
        FeedPost post,
        FeedPostReason? reason,
        IPostSubmissionService postSubmissionService,
        IDialogService dialogService,
        ILocalizer localizer,
        IAuthorViewModelFactory authorFactory)
    {
        Post = post;
        _reason = reason;
        AuthorViewModel = authorFactory.Create(post.Author);
        _postSubmissionService = postSubmissionService;
        _dialogService = dialogService;
        _localizer = localizer;
        
        IsLiked = post.Viewer?.Like is not null;
        IsReposted = post.Viewer?.Repost is not null;
        ReplyCount = post.GetReplyCount();
        RepostCount = post.GetRepostCount();
        LikeCount = post.GetLikeCount();
    }

    public AuthorViewModel AuthorViewModel { get; }

    public FeedPost Post { get; }

    public string TimeSinceCreation
    {
        get
        {
            var now = DateTime.Now;

            if (Post.Record?.CreatedAtUtc.ToLocalTime() is not DateTime createdAt ||
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
        record.Type.GetRecordType() is not RecordType.StarterPack
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

    [RelayCommand]
    private async Task ReplyAsync()
    {
        await _dialogService.OpenReplyDialogAsync(Post);
    }

    [RelayCommand]
    private async Task LikeAsync()
    {
        if (IsLiked)
        {
            return;
        }

        var result = await _postSubmissionService.LikeOrRepostAsync(
            RecordType.Like,
            Post.Uri,
            Post.Cid);

        if (result)
        {
            LikeCount = (Post.LikeCount + 1).ToString();
        }

        IsLiked = result;
    }

    [RelayCommand]
    private async Task RepostAsync()
    {
        if (IsReposted)
        {
            return;
        }

        var result = await _postSubmissionService.LikeOrRepostAsync(
            RecordType.Repost,
            Post.Uri,
            Post.Cid);

        if (result)
        {
            RepostCount = (Post.RepostCount + 1).ToString();
        }

        IsReposted = result;
    }

    public override string ToString()
    {
        return $"{AuthorViewModel.DisplayName}: {Post.Record?.Text}";
    }
}
