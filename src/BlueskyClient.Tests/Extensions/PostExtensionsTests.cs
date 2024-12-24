using Bluesky.NET.Models;
using BlueskyClient.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueskyClient.Tests.Extensions;

public class PostExtensionsTests
{

    [Fact]
    public void Given_PostWithUri_When_GetRecordKey_Then_RecordKeyExtracted()
    {
        // Arrange
        var rkey = "3lccno2bhrs2f";
        var post = new FeedPost { Uri = $"at://did:plc:7vni7e4jabnnxrqc3njjxzj6/app.bsky.feed.post/{rkey}" };

        // Act
        var result = post.GetRecordKey();

        // Assert
        Assert.Equal(rkey, result.ToString());
    }

    [Theory]
    [InlineData("noSlashHere")]
    [InlineData("ends/with/slash/")]
    public void Given_PostWithInvalidUri_When_GetRecordKey_Then_EmptyStringRetrived(string uri)
    {
        // Arrange
        var post = new FeedPost { Uri = uri };

        // Act
        var result = post.GetRecordKey();

        // Assert
        Assert.Equal(string.Empty, result.ToString());
    }

    [Fact]
    public void Given_PostWithRepostUri_When_GetRepostRecordKey_Then_RepostRecordKeyExtracted()
    {
        // Arrange
        var rkey = "3lccno2bhrs2f";
        var post = new FeedPost { Viewer = new Viewer { Repost = $"at://did:plc:7vni7e4jabnnxrqc3njjxzj6/app.bsky.feed.post/{rkey}" } };

        // Act
        var result = post.GetRepostRecordKey();

        // Assert
        Assert.Equal(rkey, result.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("noSlashHere")]
    [InlineData("ends/with/slash/")]
    public void Given_PostWithRepostWithInvalidUri_When_GetRepostRecordKey_Then_EmptyStringRetrived(string? uri)
    {
        // Arrange
        var post = new FeedPost { Viewer = new Viewer { Repost = uri } };

        // Act
        var result = post.GetRepostRecordKey();

        // Assert
        Assert.Equal(string.Empty, result.ToString());
    }

    [Fact]
    public void Given_PostWithLikeUri_When_GetLikeRecordKey_Then_LikeRecordKeyExtracted()
    {
        // Arrange
        var rkey = "3lccno2bhrs2f";
        var post = new FeedPost { Viewer = new Viewer { Like = $"at://did:plc:7vni7e4jabnnxrqc3njjxzj6/app.bsky.feed.post/{rkey}" } };

        // Act
        var result = post.GetLikeRecordKey();

        // Assert
        Assert.Equal(rkey, result.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("noSlashHere")]
    [InlineData("ends/with/slash/")]
    public void Given_PostWithLikeWithInvalidUri_When_GetLikeRecordKey_Then_EmptyStringRetrived(string? uri)
    {
        // Arrange
        var post = new FeedPost { Viewer = new Viewer { Like = uri } };

        // Act
        var result = post.GetLikeRecordKey();

        // Assert
        Assert.Equal(string.Empty, result.ToString());
    }
}
