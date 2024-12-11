using Bluesky.NET.Models;
using BlueskyClient.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BlueskyClient.ViewModels;

public sealed class AuthorViewModelFactory : IAuthorViewModelFactory
{
    private readonly IServiceProvider _serviceProvider;

    public AuthorViewModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public AuthorViewModel CreateStub()
    {
        return CreateInternal(null);
    }

    /// <inheritdoc/>
    public AuthorViewModel Create(Author? author)
    {
        return CreateInternal(author);
    }

    private AuthorViewModel CreateInternal(Author? author)
    {
        return new AuthorViewModel(
            author,
            _serviceProvider.GetRequiredService<IProfileService>());
    }
}
