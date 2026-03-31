global using GameId = string;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace NatrixServices.Chess;

[ApiController]
[Route("api/chess")]
public class Api(DataContext DataContext) : ControllerBase
{
    [HttpPost("create-user")]
    public async Task<IActionResult> CreateUser()
    {
        UserId userId = Utility.GenerateId();
        await DataContext.UserData.AddAsync(new UserData() { UserId = userId });
        await DataContext.SaveChangesAsync();
        return Created("", userId);
    }

    [HttpPost("create-game")]
    public async Task<IActionResult> CreateGame([FromQuery] bool isPublic)
    {
        GameId gameId = Utility.GenerateId();
        DataContext.GameData.Add(new GameData() { Id = gameId, IsPublic = isPublic });
        await DataContext.SaveChangesAsync();
        return Created("", gameId);
    }

    [HttpPost("join-game/{gameId}")]
    public async Task<IActionResult> JoinGame(GameId gameId, [FromQuery] UserId userId)
    {
        GameData? game = await DataContext.GetGameData(gameId);
        UserData? user = await DataContext.GetUserData(userId);

        if (game == null)
            return NotFound("Game not found");

        if (user == null)
            return NotFound("User not found");

        if (game.Player1 == null)
            game.Player1 = userId;
        else if (game.Player2 == null)
            game.Player2 = userId;
        else
            return BadRequest("Game is full");

        user.Games.Add(gameId);

        await DataContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("move/{gameId}")]
    public async Task<IActionResult> Move(GameId gameId, [FromQuery] UserId userId, [FromBody] MoveRequest data)
    {
        GameData? game = await DataContext.GameData.FindAsync(gameId);

        if (game == null)
            return NotFound("Game not found");

        if (game.Player1 != userId && game.Player2 != userId)
            return Unauthorized("You are not a participant of this game");

        if (game.Player1 == null || game.Player2 == null)
            return BadRequest("Still waiting for players");

        try
        {
            game.Game.DoMove(data.From, data.To);
        }
        catch (Exception e) when (e is ArgumentException or IllegalMoveException)
        {
            return BadRequest(e.Message);
        }
        catch (Exception)
        {
            return StatusCode(500);
        }

        await DataContext.SaveChangesAsync();

        return Ok(game.Game);
    }
    public record MoveRequest(string From, string To);

    [HttpGet("game-info/{gameId}")]
    public async Task<IActionResult> GetInfo(GameId gameId)
    {
        GameData? game = await DataContext.GameData.FindAsync(gameId);

        if (game == null)
            return NotFound("Game not found");

        return Ok(game.Game);
    }
}