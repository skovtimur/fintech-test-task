using FintechTestTask.Domain.Entities;

namespace FintechTestTask.Application.Abstractions;

public interface IUserService
{
    public Task<UserEntity?> Get(Guid id);
    public Task<UserEntity?> GetByName(string name);

    public Task<bool> NameIsFree(string name);
    public Task Create(UserEntity user);

    public Task<bool> JoinToGame(Guid userId, GameEntity game);
    public Task ExitFromGame(UserEntity user);
}