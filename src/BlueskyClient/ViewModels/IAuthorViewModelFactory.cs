using Bluesky.NET.Models;

namespace BlueskyClient.ViewModels
{
    public interface IAuthorViewModelFactory
    {
        /// <summary>
        /// Creates an author viewmodel based on the given author.
        /// If the author is null, a stub will be created.
        /// </summary>
        AuthorViewModel Create(Author? author);

        /// <summary>
        /// Creates a stub author viewmodel.
        /// </summary>
        AuthorViewModel CreateStub();
    }
}