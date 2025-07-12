using System.Text.Json;
using AutoMapper;
using FintechTestTask.Application;
using FintechTestTask.Application.Abstractions;
using FintechTestTask.Application.Abstractions.InterfacesOfAlgorithms;
using FintechTestTask.Application.Services;
using FintechTestTask.Domain.Dtos;
using FintechTestTask.Domain.Entities;
using FintechTestTask.Domain.Models;
using FintechTestTask.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FintechTestTask.Infrastructure.Services;

public class GameService(
    IUserService userService,
    MainDbContext dbContext,
    ILogger<GameService> logger,
    ICacheService cacheService,
    IMapper mapper,
    CriticalFailureGenerator criticalFailureGenerator,
    IDistributedCache cache) : IGameService
{
    public async Task CreateNewGame(GameEntity newGame, UserEntity owner)
    {
        owner.CurrentGame = newGame;
        owner.CurrentGameId = newGame.Id;

        newGame.AddNewPlayer(owner);
        await dbContext.Games.AddAsync(newGame);
        dbContext.Users.Update(owner);

        await dbContext.SaveChangesAsync();
    }

    public async Task<GameEntity?> Get(Guid gameId)
    {
        var game = await dbContext.Games
            .Include(x => x.CircleUser)
            .Include(x => x.CrossUser)
            .Include(x => x.Moves)
            .FirstOrDefaultAsync(x => x.Id == gameId);

        return game;
    }

    public async Task<List<GamePartialDto>> GetAviableGames()
    {
        var games = await dbContext.Games
            .Include(x => x.CircleUser)
            .Include(x => x.CrossUser)
            .Include(x => x.Moves)
            .Where(x => !x.IsDeleted
                        && !x.IsFinished)
            .ToListAsync();

        return games
            .Where(x => !x.GameHasBothPlayers())
            .Select(mapper.Map<GamePartialDto>)
            .ToList();
    }

    public async Task<bool> IsCellFree(CellValueObject cell, Guid gameId)
    {
        var isOccupied = await dbContext.Moves.AnyAsync(x =>
            x.GameId == gameId
            && x.Cell.Column == cell.Column
            && x.Cell.Row == cell.Row);

        return !isOccupied;
    }


    public async Task Move(MoveEntity move)
    {
        var cellsJson = await cache.GetStringAsync($"{move.Game.Id.ToString()}");
        CellStatus[,] cells;

        if (string.IsNullOrEmpty(cellsJson))
        {
            cells = await cacheService.SaveToCash(move);
        }
        else
        {
            cells = JsonConvert.DeserializeObject<CellStatus[,]>(cellsJson)
                    ?? throw new NullReferenceException("There aren't cells in cache");

            var oldValue = cells[move.Cell.Row, move.Cell.Column];

            if (oldValue != CellStatus.Empty)
                criticalFailureGenerator.ApplicationShutdown(
                    "Ранее была проверка свободна ли ячейка, если бы не была то данный код не выполнился бы");

            cells[move.Cell.Row, move.Cell.Column] = (CellStatus)move.PlayerRole;
            await cacheService.SaveToCash(cells, move.GameId);
        }

        await dbContext.Moves.AddAsync(move);

        move.Game.CurrentTurn = move.Game.CurrentTurn == PlayerRole.Circle
            ? PlayerRole.Cross
            : PlayerRole.Circle;

        await dbContext.SaveChangesAsync();
    }

    public async Task Finish(UserEntity player, GameEntity game)
    {
        if (game.IsPlayerExisting(player.Id) == false)
            criticalFailureGenerator.ApplicationShutdown("A non-player shouldn't end the game.");

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            game.Finish();
            dbContext.Games.Update(game);
            await dbContext.SaveChangesAsync();

            //В целом одного из игроков возможно может и не быть, тк не успел зайти, а owner-первый игрок передумал играть
            if (game.CircleUser != null) await userService.ExitFromGame(game.CircleUser);
            if (game.CrossUser != null) await userService.ExitFromGame(game.CrossUser);

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            await cache.RemoveAsync(game.Id.ToString());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            await transaction.RollbackAsync();

            throw;
        }
    }

    public async Task SetWinner(GameEntity game, UserEntity user)
    {
        if (game.IsPlayerExisting(user.Id) == false)
            criticalFailureGenerator.ApplicationShutdown($"Player can't be a winner. Game ID: {game.Id}");

        game.WinnerPlayerId = user.Id;
        dbContext.Update(game);
        await dbContext.SaveChangesAsync();
        
        await Finish(user, game);
    }

    public async Task SetADraw(GameEntity game, UserEntity user)
    {
        if (game.IsPlayerExisting(user.Id) == false)
            criticalFailureGenerator.ApplicationShutdown($"Player can't change the game which he aren't in. Game ID: {game.Id}");

        game.IsItDraw = true;
        
        dbContext.Update(game);
        await dbContext.SaveChangesAsync();
        
        await Finish(user, game);
    }
}