using Microsoft.AspNetCore.Mvc;
using NatrixServices.Chess.Core;
using NatrixServices.Users;

namespace NatrixServices.Chess.API;

[ApiController]
[Route("api/chess/games")]
public class GameApi(IGameManager GameManager) : ControllerBase
{
    [HttpGet("{gameId}")]
    [NoAuth]
    public async Task<IActionResult> GetGameData(GameId gameId)
    {
        var game = await GameManager.GetGameInfoAsync(gameId);
        if (game == null) return NotFound();
        return Ok(new GameDataDTO(game));
    }

    [HttpGet("{gameId}/board")]
    [NoAuth]
    public async Task<IActionResult> GetBoard(GameId gameId)
    {
        var result = await GameManager.GetFenAsync(gameId);
        return result.Match(
            onSuccess: fen => Ok(new ChessBoardDTO(new ChessGame(fen))),
            onFailure: error => error.ToActionResult()
        );
    }

    [HttpGet("{gameId}/moves")]
    [NoAuth]
    public async Task<IActionResult> GetMoves(GameId gameId)
    {
        var game = await GameManager.GetGameInfoAsync(gameId);
        if (game == null) return NotFound();
        return Ok(new MoveListDTO(game.Moves));
    }

    [HttpGet("{gameId}/allowed-moves")]
    [NoAuth]
    public async Task<IActionResult> GetAllowedMoves(GameId gameId, [FromQuery] string? field)
    {
        var result = await GameManager.GetAllowedMovesAsync(gameId, field);

        return result.Match(
            onSuccess: allowedMoves => Ok(new MoveListDTO(allowedMoves)),
            onFailure: error => error.ToActionResult()
        );
    }

    [HttpGet("")]
    [NoAuth]
    public async Task<IActionResult> GetGames([FromQuery] string? status = null, [FromQuery] string? username = null)
    {
        var games = await GameManager.GetGamesAsync(onlyPublic: true, status, username);
        return Ok(new GameListDTO(games));
    }

    [HttpPost("")]
    [NoAuth]
    public async Task<IActionResult> CreateGame([FromBody] CreateGameRequest request)
    {
        GameId gameId = await GameManager.CreateGameAsync(request.Name, request.IsPublic, request.TimePerPlayer);
        return Created($"api/chess/games/{gameId}", new { gameId });
    }
    public record CreateGameRequest(string Name, bool IsPublic, int TimePerPlayer);

    [HttpPost("{gameId}/players")]
    [AuthAsUser]
    public async Task<IActionResult> JoinGame(GameId gameId)
    {
        var result = await GameManager.JoinGameAsync(HttpContext.GetUsername(), gameId);
        return result.ToActionResult();
    }

    [HttpPost("{gameId}/moves")]
    [AuthAsUser]
    public async Task<IActionResult> Move(GameId gameId, [FromBody] MoveDTO move)
    {
        var result = await GameManager.DoMoveAsync(gameId, move.ToMove(), HttpContext.GetUsername());
        return result.ToActionResult();
    }
}