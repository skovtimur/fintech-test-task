using FintechTestTask.Application.Abstractions.InterfacesOfAlgorithms;
using FintechTestTask.Domain.Entities;
using FintechTestTask.Domain.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FintechTestTask.Application.Algorithms;

public class TicTacToeGameLogic(IDistributedCache cache, ILogger<TicTacToeGameLogic> logger) : ITicTacToeGameLogic
{
    private const int NumberOfCellsToWin = 3;
    //у диагоналей должен быть интервал n - 1, у верт и гориз просто n

    public async Task<bool> IsWinner(GameEntity gameEntity, MoveEntity moveEntity)
    {
        var jsonCells = await cache.GetStringAsync(gameEntity.Id.ToString());

        if (string.IsNullOrEmpty(jsonCells))
            return false;

        var row = moveEntity.Cell.Row;
        var column = moveEntity.Cell.Column;

        var cells = JsonConvert.DeserializeObject<CellStatus[,]>(jsonCells);

        if (cells == null)
            return false;

        var n = gameEntity.RowsAndColumsnNumber;

        var result = CheckLine(row, column, cells, 1, 0, n)
                     || CheckLine(row, column, cells, 0, 1, n)
                     || CheckLine(row, column, cells, 1, 1, n)
                     || CheckLine(row, column, cells, 1, -1, n);

        logger.LogDebug($"IsWinner({gameEntity.Id}): {result}");
        return result;
    }

    private bool CheckLine(in int row, in int column, in CellStatus[,] cells, int rowDir, int colDir,
        int columnAndRowNum)
    {
        var target = cells[row, column];
        int count = 1;

        // To Ahead
        int r = row + rowDir;
        int c = column + colDir;
        while (r >= 0 && r < columnAndRowNum
                      && c >= 0 && c < columnAndRowNum
                      && cells[r, c] == target)
        {
            count++;
            r += rowDir;
            c += colDir;
        }

        // To Back
        r = row - rowDir;
        c = column - colDir;
        while (r >= 0 && r < columnAndRowNum
                      && c >= 0 && c < columnAndRowNum
                      && cells[r, c] == target)
        {
            count++;
            r -= rowDir;
            c -= colDir;
        }
        logger.LogTrace(count.ToString());
        return count >= NumberOfCellsToWin;
    }
}