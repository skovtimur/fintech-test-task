using FintechTestTask.Domain.Validators;

namespace FintechTestTask.Domain.Entities;

public class RefreshTokenEntity : BaseEntity
{
    public string TokenHash { get; set; }

    public Guid UserId { get; set; }
    public UserEntity User { get; set; }
    public DateTime ExpiresAtUtc { get; set; }

    public static RefreshTokenEntity? Create(UserEntity user, string tokenHash, DateTime expiresAtUtc)
    {
        var newRefreshToken = new RefreshTokenEntity
        {
            User = user,
            UserId = user.Id,
            TokenHash = tokenHash,
            ExpiresAtUtc = expiresAtUtc,
        };
        return RefreshTokenValidator.IsValid(newRefreshToken) ? newRefreshToken : null;
    }
}