namespace FintechTestTask.Domain.Dtos;

public class MoveDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }

    public int Row { get; set; }
    public int Column { get; set; }
}

public class MoveDtoWitInfoAboutGame : MoveDto
{
    public Guid GameId { get; set; }
}