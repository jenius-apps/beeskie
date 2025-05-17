using Bluesky.NET.Models;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

/// <summary>
/// Interface for retrieving the entire thread of posts.
/// </summary>
public interface IPostThreadService
{
    /// <summary>
    /// Retrieves the post thread for the given AT URI post.
    /// </summary>
    /// <param name="atUri">The AT URI of the post to fetch.</param>
    /// <param name="cancellationToken">A cancel token.</param>
    /// <returns>The post thread.</returns>
    Task<PostThread?> GetThreadAsync(string atUri, CancellationToken cancellationToken);
}