using System.ComponentModel.DataAnnotations;
using AutoMapper;
using FintechTestTask.Application.Abstractions;
using FintechTestTask.Application.Abstractions.InterfacesOfAlgorithms;
using FintechTestTask.Application.Services;
using FintechTestTask.Domain.Dtos;
using FintechTestTask.Domain.Entities;
using FintechTestTask.Domain.Models;
using FintechTestTask.WebAPI.Filters;
using FintechTestTask.WebAPI.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FintechTestTask.WebAPI.Controllers;

[ApiController]
[Route("/tic-tac-toe")]
public class TicTacToeController(
    ILogger<TicTacToeController> logger,
    IUserService userService,
    ITicTacToeGameLogic gameLogic,
    IGameService gameService,
    CriticalFailureGenerator criticalFailureGenerator,
    IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Return all open games
    /// </summary>
    /// <remarks>
    /// You should have an account
    /// </remarks>
    /// <response code="200"> Return all open games </response> 
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet, Authorize, ValidationFilter]
    public async Task<IActionResult> GetActiveGames()
    {
        var games = await gameService.GetAviableGames();
        return Ok(games);
    }

    /// <summary>
    /// Get a game 
    /// </summary>
    /// <remarks>
    /// You should have an account
    /// </remarks>
    /// <response code="404"> The Game not found </response>
    /// <response code="200"> Return a found game </response> 
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("{gameId}"), Authorize, ValidationFilter]
    public async Task<IActionResult> GetGame(Guid gameId)
    {
        var gameEntity = await gameService.Get(gameId);

        if (gameEntity == null)
            return NotFound("The game was not found");

        var gameDto = mapper.Map<GameDto>(gameEntity);
        return Ok(gameDto);
    }

    /// <summary>
    /// Create new game and join the owner
    /// </summary>
    /// <response code="400"> You're already in a game </response>
    /// <response code="200"> Create and return new account</response> 
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost("create-game/{rowsAndColumnsNumber}"), Authorize, ValidationFilter]
    public async Task<IActionResult> CreateNewGame([Required, Range(3, 10)] int rowsAndColumnsNumber)
    {
        var userId = GetUserId();
        var owner = await userService.Get(userId);

        if (owner.IsLocatedInGame)
            return BadRequest(
                "You're already in a game, " +
                "but if you want to join create a new one you should finish the game in which you are");

        var newGame = GameEntity.Create(owner, rowsAndColumnsNumber);
        criticalFailureGenerator.ShutdownIfNullOrEmpty(newGame, "User could give invalid data");

        await gameService.CreateNewGame(newGame, owner);
        return Ok(newGame.Id);
    }

    /// <summary>
    /// Make a Move in game 
    /// </summary>
    /// <remarks>
    /// Simple request:
    ///     POST / CreateAccountQuery
    ///     {
    ///         "row": 2,
    ///         "column": 0,
    ///         "gameId": "e669b8e3-355d-40c4-8cac-6ccf3160e390"
    ///     }
    /// </remarks>
    /// <response code="400"> The game was finished
    /// OR The game doesn't have both players
    /// OR You've gone beyond the boundaries
    /// OR The Current Turn isn't your
    /// OR The Cell is already occupied  </response>
    /// <response code="404"> Game not found </response>
    /// <response code="403"> There aren't you in this game, so you can't move </response>
    /// <response code="200"> You make a move
    /// OR You won
    /// OR There was a draw in the game </response>
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost("move"), Authorize, ValidationFilter]
    public async Task<IActionResult> Move([Required] MoveQuery query)
    {
        //Разделить куча if-ов на мелкие фильтры-атрибуты. TODO 
        var userId = GetUserId();
        var game = await gameService.Get(query.GameId);

        if (game == null)
            return NotFound("The game was not found");

        if (game.IsFinished)
            return BadRequest("The game was finished");

        if (game.IsPlayerExisting(userId) == false)
            return Forbid();

        if (game.GameHasBothPlayers() == false)
            return BadRequest("The game doesn't have both players");

        //равно потому что отчет именно у moveEntities начинаеться с 0-ля
        //То есть пример 3x3, где (0, 0) начало и конец (2, 2)
        if (game.RowsAndColumsnNumber <= query.Column || game.RowsAndColumsnNumber <= query.Row)
            return BadRequest("You've gone beyond the boundaries");

        if ((game.CurrentTurn == PlayerRole.Circle && game.CircleUserId != userId)
            || (game.CurrentTurn == PlayerRole.Cross && game.CrossUserId != userId))
            return BadRequest("The Current Turn isn't your");

        var user = await userService.Get(userId);
        var newMove = MoveEntity.Create(game, user, row: query.Row, column: query.Column);
        criticalFailureGenerator.ShutdownIfNullOrEmpty(newMove, "User could give invalid data");

        var isFree = await gameService.IsCellFree(newMove.Cell, newMove.GameId);

        if (!isFree)
            return BadRequest("That Cell is already occupied");

        //До того как добавился новый moveEntity, нужен +1 
        int count = game.Moves.Count + 1;
        var totalCount = game.RowsAndColumsnNumber * game.RowsAndColumsnNumber;
        await gameService.Move(newMove);

        //Проверка победы
        var isWinner = await gameLogic.IsWinner(game, newMove);
        if (isWinner)
        {
            await gameService.SetWinner(game, user);

            HttpContext.Response.Headers.Append("X-Won-The-Game", "You won the game!");
            logger.LogInformation("WIN! UserId: {userId}, GameId: {gameId}", userId, game.Id);
            return Ok("You won the game!");
        }

        //Проверка ничьи
        if (count >= totalCount)
        {
            logger.LogInformation($"Draw! ({count}/{totalCount}) " +
                                  "UserId: {userId}, GameId: {gameId}", userId, game.Id);
            await gameService.SetADraw(game, user);

            HttpContext.Response.Headers.Append("X-Draw-In-Game", "Draw!");
            return Ok("Draw in the game!");
        }

        return Ok();
    }

    /// <summary>
    /// Join to an existing game
    /// </summary>
    /// <response code="400"> The game was finished
    /// OR There isn't place to join 
    /// OR You've already joined in a game </response>
    /// <response code="403"> There is you already in this game </response>
    /// <response code="404"> Game not found </response>
    /// <response code="200"> Successfully joined to the game </response>
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPatch("join-to-game/{gameId}"), Authorize, ValidationFilter]
    public async Task<IActionResult> JoinToGame([Required] Guid gameId)
    {
        var userId = GetUserId();
        var game = await gameService.Get(gameId);

        if (game == null)
            return NotFound("Game not found");

        if (game.IsFinished)
            return BadRequest("The game was finished");

        if (game.IsPlayerExisting(userId))
            return Forbid();

        if (game.GameHasBothPlayers())
            return BadRequest("There isn't place to join the game");

        var isSuccess = await userService.JoinToGame(userId, game);

        return isSuccess
            ? Ok()
            : BadRequest("You've already joined in a game");
    }

    /// <summary>
    /// Exit and finish game
    /// </summary>
    /// <response code="400"> You're not in a game </response>
    /// <response code="403"> You can't exit from the game because you aren't a member of the game </response>
    /// <response code="200"> Finish the game and exited from there </response>
    [HttpPatch("exit-from-game"), Authorize, ValidationFilter]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ExitFromGame()
    {
        var userId = GetUserId();
        var user = await userService.Get(userId);

        criticalFailureGenerator.ShutdownIfNullOrEmpty(user, $"User not found. User ID: {userId}");

        if (user?.CurrentGame?.Id == null)
            return BadRequest("You're not in a game");

        var game = await gameService.Get(user.CurrentGame.Id);
        criticalFailureGenerator.ShutdownIfNullOrEmpty(game, $"User shouldn't save non-existent game");

        if (game.IsPlayerExisting(user.Id) == false)
            return Forbid();

        await gameService.Finish(user, game);
        return Ok();
    }

    private Guid GetUserId() => User.Claims.GetUserId() ??
                                throw new InvalidOperationException("User Id shouldn't be empty in the jwt token");
}