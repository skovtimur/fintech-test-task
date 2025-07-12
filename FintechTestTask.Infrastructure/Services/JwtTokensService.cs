using FintechTestTask.Application.Abstractions;
using FintechTestTask.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FintechTestTask.Infrastructure.Services;

public class JwtTokensService(IUserService userService, MainDbContext dbContext) : IJwtTokensService
{
    public async Task<RefreshTokenEntity?> Get(string tokenHash)
    {
        return await dbContext.RefreshTokens
            .Include(token => token.User)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash);
    }

    public async Task<RefreshTokenEntity?> GetByUserId(Guid userId)
    {
        return await dbContext.RefreshTokens
            .Include(token => token.User)
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }


    public async Task UpdateRefreshToken(RefreshTokenEntity token, string newTokenHash, DateTime newExpiresAtUtc)
    {
        token.TokenHash = newTokenHash;
        token.ExpiresAtUtc = newExpiresAtUtc;

        dbContext.RefreshTokens.Update(token);
        await dbContext.SaveChangesAsync();
    }

    public async Task AddRefreshToken(RefreshTokenEntity refreshTokenEntity)
    {
        await dbContext.RefreshTokens.AddAsync(refreshTokenEntity);
        await dbContext.SaveChangesAsync();
    }
}