using BlueskyClient.ViewModels;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace BlueskyClient.Controls;

public sealed partial class PostThreadControl : UserControl
{
    public PostThreadControl()
    {
        this.InitializeComponent();

    }

    public PostThreadViewModel ViewModel { get; }
}
