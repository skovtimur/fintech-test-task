using FintechTestTask.Domain.Dtos;
using FintechTestTask.Domain.Entities;
using FintechTestTask.Domain.Models;
using FintechTestTask.Domain.ValueObjects;

namespace FintechTestTask.Application.Abstractions;

public interface IGameService
{
    public Task CreateNewGame(GameEntity newGame, UserEntity user);

    public Task<GameEntity?> Get(Guid gameId);
    public Task<List<GamePartialDto>> GetAviableGames();

    public Task Move(MoveEntity move);
    public Task<bool> IsCellFree(CellValueObject cell, Guid gameId);
    public Task Finish(UserEntity user, GameEntity game);
    public Task SetWinner(GameEntity game, UserEntity user);
    public Task SetADraw(GameEntity game, UserEntity user);
}