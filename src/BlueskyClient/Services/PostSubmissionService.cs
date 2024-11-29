using Bluesky.NET.ApiClients;
using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using BlueskyClient.Constants;
using FluentResults;
using JeniusApps.Common.Settings;
using JeniusApps.Common.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

public class PostSubmissionService : IPostSubmissionService
{
    private readonly IBlueskyApiClient _blueskyApiClient;
    private readonly IUserSettings _userSettings;
    private readonly IAuthenticationService _authenticationService;
    private readonly ITelemetry _telemetry;
    private readonly IUploadBlobService _uploadBlobService;

    public event EventHandler<(SubmissionRecord, CreateRecordResponse)>? RecordCreated;

    public PostSubmissionService(
        IBlueskyApiClient blueskyApiClient,
        IUserSettings userSettings,
        IAuthenticationService authenticationService,
        ITelemetry telemetry,
        IUploadBlobService uploadBlobService)
    {
        _blueskyApiClient = blueskyApiClient;
        _userSettings = userSettings;
        _authenticationService = authenticationService;
        _telemetry = telemetry;
        _uploadBlobService = uploadBlobService;
    }

    /// <inheritdoc/>
    public async Task<string?> ReplyAsync(string text, FeedPost parent)
    {
        text = text.Trim();
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        var root = parent.Record?.Reply?.Root is FeedPost existingRoot
            ? existingRoot
            : new FeedPost
            {
                Cid = parent.Cid,
                Uri = parent.Uri
            };

        SubmissionRecord newRecord = new()
        {
            CreatedAt = DateTime.Now,
            Text = text,
            Reply = new ReplyRecord
            {
                Parent = new FeedPost()
                {
                    Cid = parent.Cid,
                    Uri = parent.Uri
                },
                Root = root
            }
        };

        var response = await SubmitAsync(newRecord, RecordType.Reply);
        return response?.Uri;
    }

    /// <inheritdoc/>
    public async Task<bool> LikeOrRepostAsync(RecordType recordType, string targetUri, string targetCid)
    {
        if ((recordType is not RecordType.Like && recordType is not RecordType.Repost) ||
            string.IsNullOrEmpty(targetUri) ||
            string.IsNullOrEmpty(targetCid))
        {
            return false;
        }

        SubmissionRecord newRecord = new()
        {
            CreatedAt = DateTime.Now,
            Subject = new FeedPost
            {
                Uri = targetUri,
                Cid = targetCid
            }
        };

        var response = await SubmitAsync(newRecord, recordType);
        return response is not null;
    }

    /// <inheritdoc/>
    public bool ValidatePost(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return false;
        }

        if (text.Length > 300)
        {
            return false;
        }

        return true;
    }

    /// <inheritdoc/>
    public async Task<string?> SubmitPostAsync(string text)
    {
        text = text.Trim();
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        SubmissionRecord newRecord = new()
        {
            CreatedAt = DateTime.Now,
            Text = text
        };

        var response = await SubmitAsync(newRecord, RecordType.NewPost);
        return response?.Uri;
    }

    public async Task<string?> SubmitPostWithImagesAsync(string text, IReadOnlyList<string> pathsToImages)
    {
        text = text.Trim();
        IReadOnlyList<Blob?> blobs = await _uploadBlobService.UploadBlobsAsync(pathsToImages, "image/jpeg");

        SubmissionRecord newRecord = new()
        {
            CreatedAt = DateTime.Now,
            Text = text,
            Embed = new SubmissionEmbed
            {
                Type = EmbedTypes.Images,
                Images = blobs.Select(blob => new SubmissionImageBlob
                {
                    Image = blob
                }).ToArray()
            }
        };

        var response = await SubmitAsync(newRecord, RecordType.NewPost);
        return response?.Uri;
    }

    private async Task<CreateRecordResponse?> SubmitAsync(SubmissionRecord record, RecordType recordType)
    {
        Result<string> tokenResult = await _authenticationService.TryGetFreshTokenAsync();
        var handle = _userSettings.Get<string>(UserSettingsConstants.SignedInDIDKey);

        if (tokenResult.IsFailed || handle is null)
        {
            return null;
        }

        CreateRecordResponse? result = null;

        try
        {
            result = await _blueskyApiClient.SubmitPostAsync(tokenResult.Value, handle, record, recordType);
        }
        catch (Exception e)
        {
            var dict = new Dictionary<string, string>
            {
                { "method", "SubmitAsync" },
                { "recordType", recordType.ToString() },
                { "message", e.Message },
            };
            _telemetry.TrackEvent(TelemetryConstants.ApiError, dict);
            _telemetry.TrackError(e, dict);
        }

        if (result is not null)
        {
            RecordCreated?.Invoke(this, (record, result));
        }

        return result;
    }
}
