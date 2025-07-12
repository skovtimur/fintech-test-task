namespace FintechTestTask.Domain.Dtos;

public class JwtTokensDto(string refreshToken, string accessToken, DateTime expiresAtUtc)
{
    public string RefreshToken { get; set; } = refreshToken;
    public string AccessToken { get; set; } = accessToken;
    public string ExpiresAtUtc { get; set; } = expiresAtUtc.ToString();
}