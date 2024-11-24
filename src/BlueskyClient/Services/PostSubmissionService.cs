using Bluesky.NET.ApiClients;
using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using BlueskyClient.Constants;
using JeniusApps.Common.Settings;
using JeniusApps.Common.Telemetry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

public class PostSubmissionService : IPostSubmissionService
{
    private readonly IBlueskyApiClient _blueskyApiClient;
    private readonly IUserSettings _userSettings;
    private readonly IAuthenticationService _authenticationService;
    private readonly ITelemetry _telemetry;

    public event EventHandler<(SubmissionRecord, CreateRecordResponse)>? RecordCreated;

    public PostSubmissionService(
        IBlueskyApiClient blueskyApiClient,
        IUserSettings userSettings,
        IAuthenticationService authenticationService,
        ITelemetry telemetry)
    {
        _blueskyApiClient = blueskyApiClient;
        _userSettings = userSettings;
        _authenticationService = authenticationService;
        _telemetry = telemetry;
    }

    /// <inheritdoc/>
    public async Task<string?> ReplyAsync(string text, FeedPost parent)
    {
        text = text.Trim();
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        var token = await _authenticationService.TryGetFreshTokenAsync();
        var handle = _userSettings.Get<string>(UserSettingsConstants.SignedInDIDKey);

        if (token is null || handle is null)
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

        CreateRecordResponse? result = null;

        try
        {
            result = await _blueskyApiClient.SubmitPostAsync(token, handle, newRecord, RecordType.Reply);
        }
        catch (Exception e)
        {
            var dict = new Dictionary<string, string>
            {
                { "method", "SubmitPostAsync" },
                { "recordType", "Reply" },
                { "message", e.Message },
            };
            _telemetry.TrackError(e, dict);
            _telemetry.TrackEvent(TelemetryConstants.ApiError, dict);
        }

        if (result is not null)
        {
            RecordCreated?.Invoke(this, (newRecord, result));
        }

        return result?.Uri;
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

        var token = await _authenticationService.TryGetFreshTokenAsync();
        var handle = _userSettings.Get<string>(UserSettingsConstants.SignedInDIDKey);

        if (token is null || handle is null)
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

        CreateRecordResponse? result = null;

        try
        {
            result = await _blueskyApiClient.SubmitPostAsync(token, handle, newRecord, recordType);
        }
        catch (Exception e)
        {
            var dict = new Dictionary<string, string>
            {
                { "method", "SubmitPostAsync" },
                { "recordType", recordType.ToString() },
                { "message", e.Message },
            };
            _telemetry.TrackError(e, dict);
            _telemetry.TrackEvent(TelemetryConstants.ApiError, dict);
        }

        if (result is not null)
        {
            RecordCreated?.Invoke(this, (newRecord, result));
        }

        return result is not null;
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

        var token = await _authenticationService.TryGetFreshTokenAsync();
        var handle = _userSettings.Get<string>(UserSettingsConstants.SignedInDIDKey);

        if (token is null || handle is null)
        {
            return null;
        }


        SubmissionRecord newRecord = new()
        {
            CreatedAt = DateTime.Now,
            Text = text
        };

        CreateRecordResponse? result = null;

        try
        {
            result = await _blueskyApiClient.SubmitPostAsync(token, handle, newRecord, RecordType.NewPost);
        }
        catch (Exception e)
        {
            var dict = new Dictionary<string, string>
            {
                { "method", "SubmitPostAsync" },
                { "recordType", "NewPost" },
                { "message", e.Message },
            };
            _telemetry.TrackError(e, dict);
            _telemetry.TrackEvent(TelemetryConstants.ApiError, dict);
        }

        if (result is not null)
        {
            RecordCreated?.Invoke(this, (newRecord, result));
        }

        return result?.Uri;
    }
}
