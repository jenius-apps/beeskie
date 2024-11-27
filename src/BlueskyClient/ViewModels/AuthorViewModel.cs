using Bluesky.NET.Models;
using BlueskyClient.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace BlueskyClient.ViewModels;

public partial class AuthorViewModel : ObservableObject
{
    private Author? _author;

    public string DisplayName => _author?.DisplayName ?? _author?.Handle ?? string.Empty;

    public string AtHandle => _author?.AtHandle ?? string.Empty;

    public string Handle => _author?.Handle ?? string.Empty;

    public string Description => _author?.Description ?? string.Empty;

    public string AvatarUrl => _author.SafeAvatarUrl();

    public string BannerUrl => _author.SafeAvatarUrl();

    public Uri BannerUri => _author.SafeBannerUri();

    public string FollowersCount => _author.FollowersCount();

    public string FollowingCount => _author.FollowsCount();

    public string PostsCount => _author.PostsCount();

    public bool BannerVisible => !string.IsNullOrEmpty(_author?.Banner);

    public void SetAuthor(Author? author)
    {
        _author = author;
        OnPropertyChanged(string.Empty);
    }
}
