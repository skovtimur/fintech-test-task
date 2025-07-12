using System.ComponentModel.DataAnnotations;

namespace FintechTestTask.WebAPI.Queries;

public class MoveQuery
{
    [Range(0, int.MaxValue)] public  int Row { get; set; }
    [Range(0, int.MaxValue)] public int Column { get; set; }
    [Required] public Guid GameId { get; set; }
}