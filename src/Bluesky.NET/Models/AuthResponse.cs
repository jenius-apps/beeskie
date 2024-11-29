namespace Bluesky.NET.Models;

public class AuthResponse
{
    public string? Did { get; init; }
    public string? Handle { get; set; }
    public string? Email { get; set; }
    public bool? EmailConfirmed { get; set; }
    public bool? EmailAuthFactor { get; set; }
    public string? AccessJwt { get; set; }
    public string? RefreshJwt { get; set; }
    public bool? Active { get; set; }
}
