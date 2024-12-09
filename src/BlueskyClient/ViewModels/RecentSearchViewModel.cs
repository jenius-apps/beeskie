using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BlueskyClient.ViewModels;

public partial class RecentSearchViewModel : ObservableObject
{
    public RecentSearchViewModel(
        string recentSearch,
        IRelayCommand<string> runQueryCommand,
        IRelayCommand<RecentSearchViewModel> deleteQueryCommand)
    {
        Query = recentSearch;
        RunQueryCommand = runQueryCommand;
        DeleteCommand = deleteQueryCommand;
    }

    /// <summary>
    /// The query to display on screen.
    /// </summary>
    public string Query { get; }

    /// <summary>
    /// Command to run this query.
    /// </summary>
    public IRelayCommand<string> RunQueryCommand { get; }

    /// <summary>
    /// Command to delete this query from the user's history.
    /// </summary>
    public IRelayCommand<RecentSearchViewModel> DeleteCommand { get; }

    /// <inheritdoc/>
    public override string ToString() => Query;
}
