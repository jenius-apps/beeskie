using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using JeniusApps.Common.Tools;
using System;
using System.Diagnostics.CodeAnalysis;

namespace BlueskyClient.ViewModels;

public partial class NotificationViewModel : ObservableObject
{
    private readonly ILocalizer _localizer;

    public NotificationViewModel(
        Notification notification,
        ILocalizer localizer)
    {
        Notification = notification;
        _localizer = localizer;
    }

    public Notification Notification { get; }

    public bool AvatarValid => IsAvatarValid(Notification.Author);

    public string SafeAvatarUrl => IsAvatarValid(Notification.Author) ? Notification.Author.Avatar : "http://local";

    public string Reason => Notification.Reason;

    public string CaptionString
    {
        get
        {
            if (IsAvatarValid(Notification.Author) && Notification.Author.DisplayName is string displayName)
            {
                return Reason switch
                {
                    ReasonConstants.Follow => _localizer.GetString("NotificationsFollowedText", displayName),
                    ReasonConstants.Like => _localizer.GetString("NotificationsLikedText", displayName),
                    ReasonConstants.Repost => _localizer.GetString("NotificationsRepostedText", displayName),
                    ReasonConstants.Reply => _localizer.GetString("PostedReplyText"),
                    _ => string.Empty
                };
            }

            return string.Empty;
        }
    }

    public bool IsLike => Reason is ReasonConstants.Like;

    public bool IsRepost => Reason is ReasonConstants.Repost;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SubjectText))]
    private FeedPost? _subjectPost;

    public string SubjectText => SubjectPost?.Record?.Text ?? string.Empty;

    private bool IsAvatarValid([NotNullWhen(true)] Author? author) =>
        author?.Avatar is string avatarUrl &&
        Uri.IsWellFormedUriString(avatarUrl, UriKind.Absolute);
}
