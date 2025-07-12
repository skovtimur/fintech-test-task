using FintechTestTask.Domain.Entities;

namespace FintechTestTask.Application.Abstractions;

public interface IJwtTokensService
{
    Task<RefreshTokenEntity?> GetByUserId(Guid userId);
    Task<RefreshTokenEntity?> Get(string tokenHash);
    
    Task UpdateRefreshToken(RefreshTokenEntity token, string newTokenHash, DateTime newExpiresAtUtc);
    Task AddRefreshToken(RefreshTokenEntity refreshTokenEntity);
}