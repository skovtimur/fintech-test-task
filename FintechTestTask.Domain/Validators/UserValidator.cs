using FintechTestTask.Domain.Entities;
using FluentValidation;

namespace FintechTestTask.Domain.Validators;

public class UserValidator : AbstractValidator<UserEntity>
{
    public UserValidator()
    {
        RuleFor(x => x.Name).Must(x => x.Length is > 2 and <= 24);
        RuleFor(x => x.HashPassword).NotEmpty().NotNull();
    }

    private static readonly UserValidator _validator = new();
    public static bool IsValid(UserEntity entity) => _validator.Validate(entity).IsValid;
}