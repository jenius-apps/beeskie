using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using BlueskyClient.Extensions;
using BlueskyClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace BlueskyClient.ViewModels;

public partial class FeedItemViewModel : ObservableObject
{
    private readonly IPostSubmissionService _postSubmissionService;
    private readonly IDialogService _dialogService;

    public FeedItemViewModel(
        FeedItem feedItem,
        IPostSubmissionService postSubmissionService,
        IDialogService dialogService)
    {
        FeedItem = feedItem;
        _postSubmissionService = postSubmissionService;
        _dialogService = dialogService;
        
        IsLiked = feedItem.Post.Viewer?.Like is not null;
        IsReposted = feedItem.Post.Viewer?.Repost is not null;
        ReplyCount = feedItem.Post.GetReplyCount();
        RepostCount = feedItem.Post.GetRepostCount();
        LikeCount = feedItem.Post.GetLikeCount();
    }

    public FeedItem FeedItem { get; }

    public bool IsRepost => FeedItem.Reason?.Type.EndsWith("#reasonRepost", StringComparison.OrdinalIgnoreCase) ?? false;

    public string ReposterName => IsRepost
        ? FeedItem.Reason?.By?.DisplayName ?? string.Empty
        : string.Empty;

    public string RepostCaption => IsRepost
        ? $"Reposted by {ReposterName}"
        : string.Empty;

    public PostEmbed? PostEmbed => FeedItem.Post?.Embed;

    public FeedRecord? QuotedPost => 
        (FeedItem.Post?.Embed?.Record?.Record ?? FeedItem.Post?.Embed?.Record) is FeedRecord record &&
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
        await _dialogService.OpenReplyDialogAsync(FeedItem.Post);
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
            FeedItem.Post.Uri,
            FeedItem.Post.Cid);

        if (result)
        {
            LikeCount = (FeedItem.Post.LikeCount + 1).ToString();
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
            FeedItem.Post.Uri,
            FeedItem.Post.Cid);

        if (result)
        {
            RepostCount = (FeedItem.Post.RepostCount + 1).ToString();
        }

        IsReposted = result;
    }
}
