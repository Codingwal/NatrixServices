global using GameId = string;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NatrixServices.Chess;

[ApiController]
[Route("api/chess/games")]
public class GameApi(DataContext DataContext) : ControllerBase
{
    [HttpGet("{gameId}")]
    public async Task<IActionResult> GetGameData(GameId gameId)
    {
        GameData? gameData = await DataContext.GameData.FindAsync(gameId);
        if (gameData == null) return NotFound("Game not found");

        return Ok(gameData);
    }

    [HttpGet("{gameId}/board")]
    public async Task<IActionResult> GetBoard(GameId gameId)
    {
        GameData? gameData = await DataContext.GameData.FindAsync(gameId);
        if (gameData == null) return NotFound("Game not found");

        ChessGame game = new(gameData.Fen);

        return Ok(new ChessBoardDTO(game));
    }

    [HttpGet("{gameId}/moves")]
    public async Task<IActionResult> GetMoves(GameId gameId)
    {
        GameData? gameData = await DataContext.GameData.FindAsync(gameId);
        if (gameData == null) return NotFound("Game not found");

        return Ok(new MoveListDTO(gameData.Moves));
    }

    [HttpGet("{gameId}/allowed-moves")]
    public async Task<IActionResult> GetAllowedMoves(GameId gameId, [FromQuery] string? field)
    {
        GameData? gameData = await DataContext.GameData.FindAsync(gameId);
        if (gameData == null) return NotFound("Game not found");

        ChessGame game = new(gameData.Fen);
        ChessEngine engine = new(game);

        List<MoveDTO> allowedMoves;
        if (field != null)
            allowedMoves = engine.GetAllowedMoves(field);
        else
            allowedMoves = engine.GetAllowedMoves();

        return Ok(new MoveListDTO(allowedMoves));
    }

    [HttpGet("games")]
    public async Task<IActionResult> GetGames([FromQuery] bool onlyActive = true, [FromQuery] string? username = null)
    {
        List<GameData> games;

        if (onlyActive)
            games = await DataContext.GameData.Where(g => g.IsPublic && g.Player1 != null && g.Player2 != null && g.Result == null).ToListAsync();
        else
            games = await DataContext.GameData.Where(g => g.IsPublic).ToListAsync();

        if (username != null)
            games = games.Where(g => g.Player1 == username || g.Player2 == username).ToList();

        return Ok(new GameListDTO(games));
    }

    [HttpPost("games")]
    public async Task<IActionResult> CreateGame([FromBody] CreateGameRequest request)
    {
        GameId gameId = Utility.GenerateId();
        DataContext.GameData.Add(new GameData(gameId, request.IsPublic, request.TimePerPlayer, ChessGame.DefaultFen));
        await DataContext.SaveChangesAsync();
        return Created($"api/chess/games/{gameId}", gameId);
    }
    public record CreateGameRequest(bool IsPublic, int TimePerPlayer);

    [HttpPost("games/{gameId}/players")]
    [HeaderAuth]
    public async Task<IActionResult> JoinGame(GameId gameId, [FromHeader] string username)
    {
        GameData? gameData = await DataContext.GetGameData(gameId);
        if (gameData == null) return NotFound("Game not found");

        if (gameData.Player1 == null)
            gameData.Player1 = username;
        else if (gameData.Player2 == null)
            gameData.Player2 = username;
        else
            return BadRequest("Game is full");

        await DataContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("games/{gameId}/moves")]
    [HeaderAuth]
    public async Task<IActionResult> Move(GameId gameId, [FromHeader] string username, [FromBody] MoveDTO move)
    {
        GameData? gameData = await DataContext.GameData.FindAsync(gameId);
        if (gameData == null) return NotFound("Game not found");

        if (gameData.Result != null)
            return BadRequest("Game is already finished");

        if (gameData.Player1 == null || gameData.Player2 == null)
            return BadRequest("Still waiting for players");

        int playerIndex = -1;
        if (gameData.Player1 == username)
            playerIndex = 1;
        else if (gameData.Player2 == username)
            playerIndex = 2;

        if (playerIndex == -1)
            return Unauthorized("You are not a participant of this game");

        ChessGame game = new(gameData.Fen);
        ChessEngine engine = new(game);

        string? error = engine.DoMove(move, out char? result);

        if (error != null)
            return BadRequest(error);

        // Handle time
        if (gameData.LastMoveTime != default)
        {
            TimeSpan timeElapsed = DateTimeOffset.UtcNow - gameData.LastMoveTime;
            if (playerIndex == 1)
            {
                gameData.TimeLeft1 -= timeElapsed;
                if (gameData.TimeLeft1 <= TimeSpan.Zero)
                    gameData.Result = 'b'; // Player 1 loses
            }
            else
            {
                gameData.TimeLeft2 -= timeElapsed;
                if (gameData.TimeLeft2 <= TimeSpan.Zero)
                    gameData.Result = 'w'; // Player 2 loses
            }
        }

        // Update game data
        gameData.Result = result;
        gameData.LastMoveTime = DateTimeOffset.UtcNow;
        gameData.Moves.Add(move);
        gameData.Fen = game.ToFen();
        await DataContext.SaveChangesAsync();

        return Ok();
    }
}