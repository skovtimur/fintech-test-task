using FintechTestTask.Domain.Entities;
using FluentValidation;

namespace FintechTestTask.Domain.Validators;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenEntity>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.TokenHash).NotEmpty().NotNull();
        RuleFor(x => x.UserId).NotEmpty().NotNull();
        RuleFor(x => x.ExpiresAtUtc).NotEmpty().NotNull();
    }

    private static readonly RefreshTokenValidator _validator = new();
    public static bool IsValid(RefreshTokenEntity entity) => _validator.Validate(entity).IsValid;
}