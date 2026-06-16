using Microsoft.AspNetCore.Mvc;
using NatrixServices.Chess.Application.Commands;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Chess.Presentation.DTOs;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;
using NatrixServices.Shared.Infrastructure;

namespace NatrixServices.Chess.Presentation.Controllers;

[ApiController]
[Route("api/chess/games")]
public class GameController(ICommandDispatcher dispatcher) : ControllerBase
{
    [HttpGet("{gameId}")]
    [NoAuth]
    public async Task<IActionResult> GetGameData(GameId gameId)
    {
        GetGameCommand command = new(gameId);

        return await dispatcher.ExecuteCommandAsync<GetGameCommand, ChessGame>(command)
            .Map(game => new GameDataDTO(game))
            .ToActionResult();
    }

    [HttpGet("{gameId}/board")]
    [NoAuth]
    public async Task<IActionResult> GetBoard(GameId gameId)
    {
        GetGameCommand command = new(gameId);

        return await dispatcher.ExecuteCommandAsync<GetGameCommand, ChessGame>(command)
            .Map(game => Fen.FenToBoard(game.Fen))
            .Map(board => new ChessBoardDTO(board))
            .ToActionResult();
    }

    [HttpGet("{gameId}/moves")]
    [NoAuth]
    public async Task<IActionResult> GetMoves(GameId gameId)
    {
        GetGameCommand command = new(gameId);

        return await dispatcher.ExecuteCommandAsync<GetGameCommand, ChessGame>(command)
            .Map(game => new MoveListDTO(game.Moves))
            .ToActionResult();
    }

    [HttpGet("{gameId}/allowed-moves")]
    [NoAuth]
    public async Task<IActionResult> GetAllowedMoves(GameId gameId, [FromQuery] string? field)
    {
        Int2? startPos;
        if (field == null)
            startPos = null;
        else
        {
            var result = Fen.FieldDescToPos(field);
            if (result.IsFailure) return result.ToActionResult();
            startPos = result.Value;
        }

        GetAllowedMovesCommand command = new(gameId, startPos);

        return await dispatcher.ExecuteCommandAsync<GetAllowedMovesCommand, IEnumerable<Move>>(command)
            .Map(moves => new MoveListDTO(moves))
            .ToActionResult();
    }

    [HttpGet("")]
    [NoAuth]
    public async Task<IActionResult> GetGames([FromQuery] string? status = null, [FromQuery] string? username = null)
    {
        GameStatus? gameStatus;
        if (status == null)
            gameStatus = null;
        else
        {
            var res = GameStatusDTO.StrToStatus(status);
            if (res.IsFailure) return res.Error.ToActionResult();
            gameStatus = res.Value;
        }

        GetGamesCommand command = new(true, gameStatus, username);

        return await dispatcher.ExecuteCommandAsync<GetGamesCommand, IEnumerable<ChessGame>>(command)
            .Map(games => new GameListDTO(games))
            .ToActionResult();
    }

    [HttpPost("")]
    [NoAuth]
    public async Task<IActionResult> CreateGame([FromBody] CreateGameRequest request)
    {
        CreateGameCommand command = new(request.Name, request.IsPublic, TimeSpan.FromMinutes(request.TimePerPlayer));

        return await dispatcher.ExecuteCommandAsync<CreateGameCommand, GameId>(command)
            .Match(
                onSuccess: gameId => Created($"api/chess/games/{gameId}", new { gameId }),
                onFailure: err => err.ToActionResult()
            );
    }
    public record CreateGameRequest(string Name, bool IsPublic, int TimePerPlayer);

    [HttpPost("{gameId}/players")]
    [AuthAsUser]
    public async Task<IActionResult> JoinGame(GameId gameId)
    {
        JoinGameCommand command = new(gameId, HttpContext.GetUsername());

        return await dispatcher.ExecuteCommandAsync(command).ToActionResult();
    }

    [HttpPost("{gameId}/moves")]
    [AuthAsUser]
    public async Task<IActionResult> Move(GameId gameId, [FromBody] MoveDTO moveDTO)
    {
        if (!moveDTO.ToMove().TryGetValue(out var move, out var error))
            return error.ToActionResult();

        DoMoveCommand command = new(gameId, move);

        return await dispatcher.ExecuteCommandAsync(command).ToActionResult();
    }
}