using Bluesky.NET.Models;
using BlueskyClient.Constants;
using BlueskyClient.Models;
using JeniusApps.Common.Tools;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

#nullable enable

namespace BlueskyClient.Controls;

public sealed partial class FacetTextBlock : UserControl
{
    public static readonly DependencyProperty RecordProperty = DependencyProperty.Register(
        nameof(Record),
        typeof(FeedRecord),
        typeof(FacetTextBlock),
        new PropertyMetadata(null, OnRecordChanged));

    private static void OnRecordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((FacetTextBlock)d).RenderFacets();
    }

    public FacetTextBlock()
    {
        this.InitializeComponent();
    }

    public FeedRecord? Record
    {
        get => (FeedRecord)GetValue(RecordProperty);
        set => SetValue(RecordProperty, value);
    }

    private void RenderFacets()
    {
        if (Record is null)
        {
            return;
        }

        var recordText = Record.Text;
        if (Record.Facets is Facet[] facets)
        {
            DisplayTextBlock.Inlines.Clear();

            int currentByteIndex = 0;

            foreach (var f in facets)
            {
                if (f is not { Features: { } features, Index: { }  indexData })
                {
                    continue;
                }

                if (features.FirstOrDefault()?.FeatureType is FacetFeatureType.Mention)
                {
                    // TODO add support for mentions.
                    continue;
                }

                DisplayTextBlock.Inlines.Add(new Run { Text = GetSubstring(recordText, currentByteIndex, byteLength: indexData.ByteStart - currentByteIndex) });

                var hyperlink = new Hyperlink()
                {
                    UnderlineStyle = UnderlineStyle.None
                };
                hyperlink.Click += (s, e) => OnFacetClicked(features);
                hyperlink.Inlines.Add(new Run { Text = GetSubstring(recordText, indexData.ByteStart, indexData.Length) });

                DisplayTextBlock.Inlines.Add(hyperlink);

                currentByteIndex = indexData.ByteEnd;
            }

            var remainingText = GetSubstring(recordText, currentByteIndex);
            if (!string.IsNullOrEmpty(remainingText))
            {
                DisplayTextBlock.Inlines.Add(new Run { Text = remainingText });
            }
        }
        else
        {
            DisplayTextBlock.Text = recordText;
        }
    }

    public string GetSubstring(string normalText, int byteStart, int? byteLength = null)
    {
        // The indices returned by Bluesky are based in UTF8 bytes, so we have to
        // perform index operations in the context of bytes.
        // Ref: https://docs.bsky.app/docs/advanced-guides/post-richtext#text-encoding-and-indexing.
        
        // Here we grab the normal text in the form of bytes.
        byte[] bytes = Encoding.Default.GetBytes(normalText);
        if (byteStart >= bytes.Length)
        {
            // Guard against invalid parameters.
            return string.Empty;
        }

        // We skip the bytes until we get to the desired start index byte position.
        IEnumerable<byte> byteEnumerable = bytes.Skip(byteStart);

        if (byteLength is int l)
        {
            // If a length is provided, we take the bytes within that range.
            byteEnumerable = byteEnumerable.Take(l);
        }

        // We convert the byte sub array back to a regular string.
        byte[] byteSubstring = byteEnumerable.ToArray();
        return Encoding.UTF8.GetString(byteSubstring);
    }

    private async void OnFacetClicked(IReadOnlyList<FacetFeature> features)
    {
        if (features is not [FacetFeature feature, ..])
        {
            return;
        }

        if (feature.FeatureType is FacetFeatureType.Link &&
            feature.Uri is string uri &&
            Uri.TryCreate(uri, UriKind.Absolute, out Uri result))
        {
            await Launcher.LaunchUriAsync(result);
        }
        else if (feature.FeatureType is FacetFeatureType.Tag && feature.Tag is string tag)
        {
            var contentNavigator = App.Services.GetRequiredKeyedService<INavigator>(NavigationConstants.ContentNavigatorKey);
            contentNavigator.NavigateTo(NavigationConstants.SearchPage, new SearchPageNavigationArgs
            {
                RequestedQuery = tag
            });
        }
        else if (feature.FeatureType is FacetFeatureType.Mention && feature.Did is string did)
        {
            // TODO add support for mention clicks
        }
    }
}
