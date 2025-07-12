using FintechTestTask.Domain.Entities;
using FintechTestTask.Domain.Models;

namespace FintechTestTask.Application;

public interface ICacheService
{
    public Task<CellStatus[,]> SaveToCash(MoveEntity move);
    public Task<bool> SaveToCash(CellStatus[,] cells, Guid gameId);
}