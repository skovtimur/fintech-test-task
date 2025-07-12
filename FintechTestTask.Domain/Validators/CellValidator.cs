using FintechTestTask.Domain.ValueObjects;
using FluentValidation;

namespace FintechTestTask.Domain.Validators;

public class CellValidator : AbstractValidator<CellValueObject>
{
    public CellValidator()
    {
        RuleFor(x => x.Row).Must(x => x >= 0);
        RuleFor(x => x.Column).Must(x => x >= 0);
    }
    
    private static readonly CellValidator _validator = new();
    public static bool IsValid(CellValueObject value) => _validator.Validate(value).IsValid;
}