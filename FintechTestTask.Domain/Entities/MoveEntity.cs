using FintechTestTask.Domain.Models;
using FintechTestTask.Domain.Validators;
using FintechTestTask.Domain.ValueObjects;

namespace FintechTestTask.Domain.Entities;

public class MoveEntity : BaseEntity
{
    public Guid GameId { get; set; }
    public GameEntity Game { get; set; }

    public CellValueObject Cell { get; set; }

    public UserEntity Owner { get; set; }
    public Guid OwnerId { get; set; }
    public PlayerRole PlayerRole { get; set; }

    public static MoveEntity? Create(GameEntity game, UserEntity user, int row, int column)
    {
        var newCell = CellValueObject.Create(row, column);

        if (newCell == null)
            return null;

        var newMove = new MoveEntity
        {
            GameId = game.Id,
            Game = game,
            Owner = user,
            Cell = newCell,
            PlayerRole = game.CurrentTurn
        };
        
        return MoveValidator.IsValid(newMove) ? newMove : null;
    }
}