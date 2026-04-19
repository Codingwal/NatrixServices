using Microsoft.AspNetCore.Mvc;

namespace NatrixServices.Chess;

public class UserNotFoundException() : Exception("User not found");

[ApiController]
[Route("api/chess/users")]
public class UserApi(UserDataContext DataContext, Users.DataContext UserDataContext, IGameManager GameManager) : ControllerBase
{
    [HttpGet("{username}")]
    public async Task<IActionResult> GetUserData(string username)
    {
        if (!await UserDataContext.UserExists(username)) throw new UserNotFoundException();

        var userData = await DataContext.GetOrCreateUserAsync(username);
        return Ok(new UserDataDTO(userData));
    }

    [HttpGet("{username}/games")]
    [HeaderAuth]
    public async Task<IActionResult> GetUserGames(string username, [FromQuery] string? status = null)
    {
        List<GameData> games = await GameManager.GetGamesAsync(onlyPublic: false, status, username);
        return Ok(new GameListDTO(games));
    }

    [HttpGet("{username}/stats")]
    public async Task<IActionResult> GetUserStats(string username)
    {
        if (!await UserDataContext.UserExists(username)) throw new UserNotFoundException();

        var userData = await DataContext.GetOrCreateUserAsync(username);
        return Ok(userData.Stats);
    }
}