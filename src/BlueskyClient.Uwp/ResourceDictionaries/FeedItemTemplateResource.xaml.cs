// Required to support xbind
// in resource dictionaries.
// Ref: https://docs.microsoft.com/en-us/windows/uwp/data-binding/data-binding-in-depth#resource-dictionaries-with-xbind

#nullable enable

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace BlueskyClient.ResourceDictionaries;

public sealed partial class FeedItemTemplateResource
{
    public FeedItemTemplateResource()
    {
        this.InitializeComponent();
    }

    private void OnRepostClicked(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe)
        {
            FlyoutBase.ShowAttachedFlyout(fe);
        }
    }
}
