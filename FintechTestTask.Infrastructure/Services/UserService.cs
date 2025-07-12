using FintechTestTask.Application.Abstractions;
using FintechTestTask.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FintechTestTask.Infrastructure.Services;

public class UserService(MainDbContext dbContext) : IUserService
{
    public async Task<UserEntity?> Get(Guid id)
    {
        var foundUser = await dbContext.Users
            .Include(x => x.CurrentGame)
            .FirstOrDefaultAsync(x => x.Id == id);

        return foundUser;
    }

    public async Task<UserEntity?> GetByName(string name)
    {
        var foundUser = await dbContext.Users
            .Include(x => x.CurrentGame)
            .FirstOrDefaultAsync(x => x.Name == name);

        return foundUser;
    }

    public async Task<bool> NameIsFree(string name)
    {
        var isOccupied = await dbContext.Users.AnyAsync(x => x.Name == name);
        return !isOccupied;
    }

    public async Task Create(UserEntity user)
    {
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> JoinToGame(Guid userId, GameEntity game)
    {
        var user = await Get(userId);

        if (user == null || user.IsLocatedInGame)
            return false;
        
        user.CurrentGame = game;
        user.CurrentGameId = game.Id;
        
        game.AddNewPlayer(user);
        dbContext.Games.Update(game);

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task ExitFromGame(UserEntity user)
    {
        user.ExitFromGame();
        dbContext.Users.Update(user);

        await dbContext.SaveChangesAsync();
    }
}