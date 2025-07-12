using FintechTestTask.Domain.Validators;

namespace FintechTestTask.Domain.ValueObjects;

public class CellValueObject
{
    public int Row { get; set; }
    public int Column { get; set; }

    public static CellValueObject? Create(int row, int column)
    {
        var newCell = new CellValueObject { Row = row, Column = column };
        
        return CellValidator.IsValid(newCell) ? newCell : null;
    }
}