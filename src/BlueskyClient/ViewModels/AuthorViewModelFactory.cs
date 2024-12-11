using Bluesky.NET.Models;
using BlueskyClient.Services;
using JeniusApps.Common.Telemetry;
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
    public AuthorViewModel CreateStub(string telemetryContext = "")
    {
        return CreateInternal(null, telemetryContext);
    }

    /// <inheritdoc/>
    public AuthorViewModel Create(Author? author, string telemetryContext = "")
    {
        return CreateInternal(author, telemetryContext);
    }

    private AuthorViewModel CreateInternal(Author? author, string telemetryContext)
    {
        return new AuthorViewModel(
            author,
            _serviceProvider.GetRequiredService<IProfileService>(),
            _serviceProvider.GetRequiredService<ITelemetry>(),
            telemetryContext);
    }
}
