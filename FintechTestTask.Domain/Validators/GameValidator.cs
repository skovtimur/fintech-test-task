using FintechTestTask.Domain.Entities;
using FluentValidation;

namespace FintechTestTask.Domain.Validators;

public class GameValidator : AbstractValidator<GameEntity>
{
    public GameValidator()
    {
        RuleFor(x => x.RowsAndColumsnNumber).Must(IsAllowed);
        
        RuleFor(x => x.OwnerId).NotEmpty().NotNull();
    }

    private bool IsAllowed(int value) => value is >= 3 and <= 10;

    private static readonly GameValidator _validator = new();
    public static bool IsValid(GameEntity entity) => _validator.Validate(entity).IsValid;
}