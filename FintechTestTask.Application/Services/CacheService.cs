using FintechTestTask.Domain.Entities;
using FintechTestTask.Domain.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace FintechTestTask.Application.Services;

public class CacheService(IDistributedCache cache) : ICacheService
{
    public async Task<CellStatus[,]> SaveToCash(MoveEntity move)
    {
        var cells = new CellStatus[move.Game.RowsAndColumsnNumber, move.Game.RowsAndColumsnNumber];

        for (var row = 0; row < cells.GetLength(0); row++)
        for (var column = 0; column < cells.GetLength(1); column++)
            cells[row, column] = CellStatus.Empty;

        foreach (var m in move.Game.Moves)
            cells[m.Cell.Row, m.Cell.Column] = (CellStatus)m.PlayerRole;

        cells[move.Cell.Row, move.Cell.Column] = (CellStatus)move.PlayerRole;
        
        var jsonCells = JsonConvert.SerializeObject(cells);
        await cache.SetStringAsync($"{move.Game.Id.ToString()}", jsonCells);
        return cells;
    }

    public async Task<bool> SaveToCash(CellStatus[,] cells, Guid gameId)
    {
        var jsonCells = JsonConvert.SerializeObject(cells);

        if (string.IsNullOrEmpty(jsonCells))
            return false;

        await cache.SetStringAsync($"{gameId.ToString()}", jsonCells);
        return true;
    }
}