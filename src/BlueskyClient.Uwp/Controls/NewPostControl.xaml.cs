using BlueskyClient.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace BlueskyClient.Controls;

public sealed partial class NewPostControl : UserControl
{
    public NewPostControl()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<NewPostViewModel>();
    }

    public NewPostViewModel ViewModel { get; }

    private void OnPasteDetected(object sender, TextControlPasteEventArgs e)
    {
        if (sender is not TextBox inputField)
        {
            return;
        }

        var dataPackageView = Clipboard.GetContent();
        if (dataPackageView.Contains(StandardDataFormats.Bitmap))
        {
            e.Handled = true;
            // TODO
        }
    }
}
