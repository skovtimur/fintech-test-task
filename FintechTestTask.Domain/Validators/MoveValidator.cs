using FintechTestTask.Domain.Entities;
using FluentValidation;

namespace FintechTestTask.Domain.Validators;

public class MoveValidator : AbstractValidator<MoveEntity>
{
    public MoveValidator()
    {
        RuleFor(x => x.GameId).NotEmpty().NotNull();
        RuleFor(x => x.Game).NotEmpty().NotNull();
        RuleFor(x => x.Owner).NotEmpty().NotNull();
        RuleFor(x => x.Cell).NotEmpty().NotNull();
    }

    private static readonly MoveValidator _validator = new();
    public static bool IsValid(MoveEntity entity) => _validator.Validate(entity).IsValid;
}