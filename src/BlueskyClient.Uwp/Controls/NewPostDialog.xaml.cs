using Bluesky.NET.Models;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace BlueskyClient.Controls;

public sealed partial class NewPostDialog : ContentDialog
{
    public NewPostDialog()
    {
        this.InitializeComponent();
    }

    public void Initialize(FeedPost? targetPost = null)
    {
        _ = NewPostControl.ViewModel.InitializeAsync(targetPost).ConfigureAwait(false);
    }

    private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var d = args.GetDeferral();
        this.IsPrimaryButtonEnabled = false;

        if (!NewPostControl.ViewModel.CanSubmit())
        {
            args.Cancel = true;
            d.Complete();
            this.IsPrimaryButtonEnabled = true;
            return;
        }

        await NewPostControl.ViewModel.SubmitCommand.ExecuteAsync(null);
        d.Complete();
        this.IsPrimaryButtonEnabled = true;
    }
}
