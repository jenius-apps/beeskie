using BlueskyClient.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace BlueskyClient.Controls;

public sealed partial class PostThreadControl : UserControl
{
    public PostThreadControl()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<PostThreadViewModel>();

    }

    public PostThreadViewModel ViewModel { get; }
}
