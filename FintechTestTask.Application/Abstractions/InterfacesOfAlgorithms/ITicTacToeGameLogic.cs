using FintechTestTask.Domain.Entities;

namespace FintechTestTask.Application.Abstractions.InterfacesOfAlgorithms;

public interface ITicTacToeGameLogic
{
    public Task<bool> IsWinner(GameEntity gameEntity, MoveEntity moveEntity);
}