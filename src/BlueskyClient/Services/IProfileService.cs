using Bluesky.NET.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

public interface IProfileService
{
    /// <summary>
    /// Follows the target actor.
    /// </summary>
    /// <param name="subjectDid">The unique DID for the actor.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>True if the follow was successful, false otherwise.</returns>
    Task<bool> FollowActorAsync(string subjectDid, CancellationToken ct);
    Task<Author?> GetCurrentUserAsync();

    /// <summary>
    /// Retrieves the full author information for the given author identifier.
    /// </summary>
    /// <param name="identifier">The DID for the author.</param>
    /// <param name="cancellationToken">A cancel token.</param>
    /// <returns>The fully populated author object, or null if not found.</returns>
    Task<Author?> GetFullAuthorProfileAsync(string? identifier, CancellationToken cancellationToken);

    Task<IReadOnlyList<FeedItem>> GetProfileFeedAsync(string handle);
}