using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.Caches;

public interface ICache<T>
{
    public Task<IReadOnlyDictionary<string, T>> GetItemsAsync(CancellationToken ct);

    public Task<IReadOnlyDictionary<string, T>> GetItemsAsync(IReadOnlyList<string> ids, CancellationToken ct);

    Task<T?> GetItemAsync(string id, CancellationToken ct);
}

public class CachedItem<T>
{
    public required DateTime ExpirationTime { get; init; }

    public required T Data { get; init; }
}
