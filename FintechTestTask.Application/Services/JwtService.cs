using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FintechTestTask.Application.Options;
using FintechTestTask.Domain.Entities;
using FintechTestTask.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FintechTestTask.Application.Services;

public class JwtService(
    IOptions<JwtOptions> options,
    ILogger<JwtService> logger)
{
    public const string UserIdClaimType = "userId";
    public const string UserNameClaimType = "userName";

    private readonly JwtOptions _options = options.Value;

    public JwtTokens GenerateTokens(UserEntity user)
    {
        var accessToken = AccessTokenCreate(user);
        var (refreshToken, expiresAtUtc) = RefreshTokenCreate(user);

        return new JwtTokens(refreshToken: refreshToken, accessToken: accessToken, expiresAtUtc);
    }

    private List<Claim> GetClaims(UserEntity user)
    {
        return
        [
            new Claim(UserIdClaimType, user.Id.ToString()),
            new Claim(UserNameClaimType, user.Name),
        ];
    }

    private string AccessTokenCreate(UserEntity user)
    {
        var signingCredentials = new SigningCredentials(
            _options.GetAccessSymmetricSecurityKey(), _options.AlgorithmForAccessToken);

        var claims = GetClaims(user);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_options.AccessTokenExpiresMinutes),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private (string, DateTime) RefreshTokenCreate(UserEntity user)
    {
        var expiresAtUtc = DateTime.UtcNow.AddDays(_options.RefreshTokenExpiresDays);

        var signingCredentials = new SigningCredentials(
            _options.GetRefreshAsymmetricSecurityKey(), _options.AlgorithmForRefreshToken);

        var claims = GetClaims(user);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAtUtc,
            signingCredentials: signingCredentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }
}