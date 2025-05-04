using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

public interface IPostSubmissionService
{
    event EventHandler<(SubmissionRecord, CreateRecordResponse)>? RecordCreated;

    Task<string?> LikeOrRepostAsync(RecordType recordType, string targetUri, string targetCid);
    Task<bool> LikeOrRepostUndoAsync(RecordType recordType, string targetUri, CancellationToken cancellationToken);
    Task<string?> ReplyAsync(string text, FeedPost parent);
    Task<string?> SubmitPostAsync(string text, IReadOnlyList<string>? pathsToImages = null, FeedPost? quotePost = null);
    bool ValidatePost(string text);
}