using FintechTestTask.Domain.Models;

namespace FintechTestTask.Domain.Dtos;

public class GameDto
{
    public Guid Id { get; set; }
    public int RowsAndColumsnNumber { get; set; }
    public List<MoveDto> Moves { get; set; }
    public PlayerRole CurrentTurn { get; set; }
    public Guid? WinnerPlayerId { get; set; } = null;

    public Guid OwnerId { get; set; }
    public Guid? CircleUserId { get; set; }
    public Guid? CrossUserId { get; set; }

    public bool IsItDraw { get; set; } = false;
    public bool IsFinished { get; set; } = false;
    public string? FinishedAt { get; set; } = null;
}

public class GamePartialDto
{
    public Guid Id { get; set; }
    public int RowsAndColumsnNumber { get; set; }
    public PlayerRole CurrentTurn { get; set; }

    public Guid OwnerId { get; set; }
    public Guid? CircleUserId { get; set; }
    public Guid? CrossUserId { get; set; }
}