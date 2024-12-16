using Bluesky.NET.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

public sealed class FacetService
{
    private const int HashtagMaxLength = 64; // inclusive of #
    private static readonly Lazy<Regex> _mentionRegexLazy = new(() => new Regex(@"(^|\s|\()(@)([a-zA-Z0-9.-]+)(\b)", RegexOptions.Compiled));
    private static readonly Lazy<Regex> _hashtagRegexLazy = new(() => new Regex(@"(?:^|\s)(#[^\d\s]\S*)(?=\s)?", RegexOptions.Compiled));
    private const string EndPunctuationRegexPattern = @"[^\w\s]+$";

    public async Task<IReadOnlyList<Facet>> ExtractFacetsAsync(string text, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        List<Facet> results = new();

        results.AddRange(ExtractHashtags(text));

        await Task.Delay(1);

        return results;
    }

    private static IEnumerable<Facet> ExtractHashtags(string text)
    {
        var hashtagMatches = _hashtagRegexLazy.Value.Matches(text);

        foreach (Match match in hashtagMatches)
        {
            bool hasLeadingSpace = match.Value.Length > 0 && char.IsWhiteSpace(match.Value[0]);
            int startIndex = hasLeadingSpace ? match.Index + 1 : match.Index;

            var tag = Regex.Replace(match.Value.Trim(), EndPunctuationRegexPattern, string.Empty);
            if (tag.Length > HashtagMaxLength)
            {
                continue;
            }

            Facet facet = new()
            {
                Index = new IndexData
                {
                    ByteStart = GetByteIndex(text, startIndex),
                    ByteEnd = GetByteIndex(text, startIndex + tag.Length)
                },
                Features =
                [
                    new FacetFeature
                    {
                        Type = FacetTypes.Tag,
                        Tag = tag.TrimStart('#')
                    }
                ]
            };

            yield return facet;
        }
    }

    private static int GetByteIndex(string fullNormalText, int index)
    {
        string substring = fullNormalText[..index];
        byte[] bytes = Encoding.Default.GetBytes(substring);
        return bytes.Length;
    }
}
