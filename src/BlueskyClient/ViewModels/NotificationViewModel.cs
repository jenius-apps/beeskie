using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using JeniusApps.Common.Tools;

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
        AuthorViewModel.SetAuthor(notification.Author);
    }

    public AuthorViewModel AuthorViewModel { get; } = new();

    public Notification Notification { get; }

    public string Reason => Notification.Reason;

    public string CaptionString
    {
        get
        {
            string displayName = AuthorViewModel.DisplayName;

            return Reason switch
            {
                ReasonConstants.Follow => _localizer.GetString("NotificationsFollowedText", displayName),
                ReasonConstants.Like => _localizer.GetString("NotificationsLikedText", displayName),
                ReasonConstants.Repost => _localizer.GetString("NotificationsRepostedText", displayName),
                ReasonConstants.Reply => _localizer.GetString("PostedReplyText"),
                _ => string.Empty
            };
        }
    }

    public bool IsLike => Reason is ReasonConstants.Like;

    public bool IsRepost => Reason is ReasonConstants.Repost;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SubjectText))]
    private FeedPost? _subjectPost;

    public string SubjectText => SubjectPost?.Record?.Text ?? string.Empty;

    public override string ToString()
    {
        return Reason switch
        {
            ReasonConstants.Reply => $"{AuthorViewModel.DisplayName}, {_localizer.GetString("PostedReplyText")}",
            _ => CaptionString
        };
    }
}
