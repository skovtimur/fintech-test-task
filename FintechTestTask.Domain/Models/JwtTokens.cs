namespace FintechTestTask.Domain.Models;

public readonly struct 
    JwtTokens(string refreshToken, string accessToken, DateTime expiresAtUtc)
{
    public string RefreshToken { get; } = refreshToken;
    public string AccessToken { get; } = accessToken;
    public DateTime ExpiresAtUtc { get; } = expiresAtUtc;
}