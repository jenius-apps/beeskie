using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlueskyClient.ViewModels;

public partial class FeedsPageViewModel : ObservableObject
{
    public async Task InitializeAsync()
    {
        await Task.Delay(1);
    }
}
