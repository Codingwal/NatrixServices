using Microsoft.AspNetCore.Mvc;
using NatrixServices.Chess.Core;
using NatrixServices.Chess.Data;
using NatrixServices.Chess.Management;
using NatrixServices.Users;

namespace NatrixServices.Chess.API;

[ApiController]
[Route("api/chess/games")]
public class GameApi(IGameManager GameManager) : ControllerBase
{
    [HttpGet("{gameId}")]
    public async Task<IActionResult> GetGameData(GameId gameId)
    {
        var gameData = await GameManager.GetGameDataAsync(gameId);
        return Ok(new GameDataDTO(gameData));
    }

    [HttpGet("{gameId}/board")]
    public async Task<IActionResult> GetBoard(GameId gameId)
    {
        var gameData = await GameManager.GetGameDataAsync(gameId);
        ChessGame game = new(gameData.Fen);
        return Ok(new ChessBoardDTO(game));
    }

    [HttpGet("{gameId}/moves")]
    public async Task<IActionResult> GetMoves(GameId gameId)
    {
        var gameData = await GameManager.GetGameDataAsync(gameId);
        return Ok(new MoveListDTO(gameData.Moves));
    }

    [HttpGet("{gameId}/allowed-moves")]
    public async Task<IActionResult> GetAllowedMoves(GameId gameId, [FromQuery] string? field)
    {
        var allowedMoves = await GameManager.GetAllowedMovesAsync(gameId, field);
        return Ok(new MoveListDTO(allowedMoves));
    }

    [HttpGet("")]
    public async Task<IActionResult> GetGames([FromQuery] string? status = null, [FromQuery] string? username = null)
    {
        List<GameData> games = await GameManager.GetGamesAsync(onlyPublic: true, status, username);
        return Ok(new GameListDTO(games));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateGame([FromBody] CreateGameRequest request)
    {
        GameId gameId = await GameManager.CreateGameAsync(request.Name, request.IsPublic, request.TimePerPlayer);
        return Created($"api/chess/games/{gameId}", new { gameId });
    }
    public record CreateGameRequest(string Name, bool IsPublic, int TimePerPlayer);

    [HttpPost("{gameId}/players")]
    [HeaderAuth]
    public async Task<IActionResult> JoinGame(GameId gameId, [FromHeader] string username)
    {
        await GameManager.JoinGameAsync(username, gameId);
        return Ok();
    }

    [HttpPost("{gameId}/moves")]
    [HeaderAuth]
    public async Task<IActionResult> Move(GameId gameId, [FromHeader] string username, [FromBody] MoveDTO move)
    {
        await GameManager.DoMoveAsync(gameId, move.ToMove(), username);
        return Ok();
    }
}