using Bluesky.NET.Models;
using BlueskyClient.Extensions;
using BlueskyClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueskyClient.Tests.Extensions;

public class UriStringHelperTests
{

    [Fact]
    public void Given_UriStringValid_When_GetRecordKey_Then_RecordKeyExtracted()
    {
        // Arrange
        var rkey = "3lccno2bhrs2f";
        var uriString = $"at://did:plc:7vni7e4jabnnxrqc3njjxzj6/app.bsky.feed.post/{rkey}";

        // Act
        var result = UriStringHelper.GetRecordKey(uriString);

        // Assert
        Assert.Equal(rkey, result.ToString());
    }

    [Theory]
    [InlineData("noSlashHere")]
    [InlineData("ends/with/slash/")]
    public void Given_UriStringInvalid_When_GetRecordKey_Then_EmptyStringRetrived(string uriString)
    {
        // Act
        var result = UriStringHelper.GetRecordKey(uriString);

        // Assert
        Assert.Equal("", result.ToString());
    }

}
