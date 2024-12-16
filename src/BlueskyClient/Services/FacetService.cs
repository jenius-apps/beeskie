using Bluesky.NET.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

public sealed class FacetService : IFacetService
{
    private const int HashtagMaxLength = 64; // inclusive of #
    private static readonly Lazy<Regex> _mentionRegexLazy = new(() => new Regex(@"(^|\s|\()(@)([a-zA-Z0-9.-]+)(\b)", RegexOptions.Compiled));
    private static readonly Lazy<Regex> _hashtagRegexLazy = new(() => new Regex(@"(?:^|\s)(#[^\d\s]\S*)(?=\s)?", RegexOptions.Compiled));
    private static readonly Lazy<Regex> _linkRegexLazy = new(() => new Regex(@"(^|\s|\()((https?:\/\/[\S]+)|((?<domain>[a-z][a-z0-9]*(\.[a-z0-9]+)+)[\S]*))", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled));
    private const string EndPunctuationRegexPattern = @"[^\w\s]+$";

    public async Task<IReadOnlyList<Facet>> ExtractFacetsAsync(string text, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        List<Facet> results = new();

        results.AddRange(ExtractHashtags(text));
        results.AddRange(ExtractLinks(text));

        await Task.Delay(1);

        return results;
    }

    private static IEnumerable<Facet> ExtractHashtags(string text)
    {
        var matches = _hashtagRegexLazy.Value.Matches(text);

        foreach (Match match in matches)
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

    private static IEnumerable<Facet> ExtractLinks(string text)
    {
        var matches = _linkRegexLazy.Value.Matches(text);

        foreach (Match match in matches)
        {
            Group target = match.Groups[2];
            string uri = target.Value;
            int uriLength = uri.Length;

            if (!uri.StartsWith("http"))
            {
                string domain = match.Groups["domain"].Value;
                if (!IsValidDomain(domain))
                {
                    continue;
                }

                uri = $"https://{uri}";
            }

            while (Regex.IsMatch(uri, @"[.,;!?]$"))
            {
                uri = uri[..^1];
                uriLength--;
            }

            if (Regex.IsMatch(uri, @"[)]$") && !uri.StartsWith("("))
            {
                uri = uri[..^1];
                uriLength--;
            }

            Facet facet = new()
            {
                Index = new IndexData
                {
                    ByteStart = GetByteIndex(text, target.Index),
                    ByteEnd = GetByteIndex(text, target.Index + uriLength)
                },
                Features =
                [
                    new FacetFeature
                    {
                        Type = FacetTypes.Link,
                        Uri = uri
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

    private static bool IsValidDomain(string domain)
    {
        if (string.IsNullOrEmpty(domain))
        {
            return false;
        }

        string pattern = @"^(?!:\/\/)([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}$";
        return Regex.IsMatch(domain, pattern);
    }
}
