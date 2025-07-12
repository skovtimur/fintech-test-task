using FintechTestTask.Domain.Models;
using FintechTestTask.Domain.Validators;

namespace FintechTestTask.Domain.Entities;

public class GameEntity : BaseEntity
{
    public int RowsAndColumsnNumber { get; set; }
    public List<MoveEntity> Moves { get; set; }
    public PlayerRole CurrentTurn { get; set; }

    public Guid OwnerId { get; set; }

    public UserEntity? CircleUser { get; set; }
    public Guid? CircleUserId { get; set; }

    public Guid? CrossUserId { get; set; }
    public UserEntity? CrossUser { get; set; }

    public Guid? WinnerPlayerId { get; set; } = null;
    public bool IsFinished { get; set; }
    public bool IsItDraw { get; set; } = false;
    public DateTime? FinishedAt { get; set; }


    private readonly Random _random = new();

    public static GameEntity? Create(UserEntity owner, int rowsAndColumsnNum)
    {
        var newGame = new GameEntity
        {
            Id = Guid.NewGuid(),
            RowsAndColumsnNumber = rowsAndColumsnNum,
            OwnerId = owner.Id,
            CurrentTurn = PlayerRole.Cross
        };

        return GameValidator.IsValid(newGame) ? newGame : null;
    }

    public void AddNewPlayer(UserEntity user)
    {
        if (IsPlayerExisting(user.Id))
            return;

        var randomRole = GetRandomPlayerRole();

        if (randomRole == PlayerRole.Circle)
        {
            CircleUser = user;
            CircleUserId = user.Id;
            return;
        }

        CrossUser = user;
        CrossUserId = user.Id;
    }

    private PlayerRole GetRandomPlayerRole()
    {
        var emptyUsers = new List<PlayerRole>(2);

        if (CircleUser == null || CircleUserId == Guid.Empty || CircleUserId == null)
            emptyUsers.Add(PlayerRole.Circle);

        if ((CrossUser == null || CrossUserId == Guid.Empty || CrossUserId == null))
            emptyUsers.Add(PlayerRole.Cross);

        if (emptyUsers.Count == 0)
            throw new Exception("Either user must be null");

        var randomNum = _random.Next(0, emptyUsers.Count);
        return emptyUsers[randomNum];
    }

    public void Finish()
    {
        FinishedAt = DateTime.UtcNow;
        IsFinished = true;
    }

    public bool IsPlayerExisting(Guid playerId) => CircleUserId == playerId || CrossUserId == playerId;
    public bool GameHasBothPlayers() => CircleUser != null && CrossUser != null;

    public PlayerRole? GetRoleOfUser(UserEntity user)
    {
        PlayerRole? role = null;

        if (user.Id == CircleUserId)
            role = PlayerRole.Circle;

        else if (user.Id == CrossUserId)
            role = PlayerRole.Cross;

        return role;
    }
}