using FintechTestTask.Domain.Validators;

namespace FintechTestTask.Domain.Entities;

public class UserEntity : BaseEntity
{
    public string Name { get; set; }
    public string HashPassword { get; set; }

    public Guid? CurrentGameId { get; set; } = null;
    public GameEntity? CurrentGame { get; set; } = null;

    public static UserEntity? Create(string name, string hashPassword)
    {
        var newUser = new UserEntity { Name = name, HashPassword = hashPassword };
        return UserValidator.IsValid(newUser) ? newUser : null;
    }

    public bool IsLocatedInGame => CurrentGameId != null || CurrentGame != null;
    public void ExitFromGame()
    {
        CurrentGame = null;
        CurrentGameId = null;
    }
}