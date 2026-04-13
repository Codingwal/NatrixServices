using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NatrixServices.Chess;

[ApiController]
[Route("api/chess/users")]
public class UserApi(DataContext DataContext, Users.DataContext UserDataContext) : ControllerBase
{
    [HttpGet("{username}")]
    public async Task<IActionResult> GetUserData(string username)
    {
        if (!await UserDataContext.UserExists(username)) return NotFound();

        var userData = await DataContext.GetUserData(username);
        return Ok(new UserDataDTO(userData));
    }

    [HttpGet("{username}/games")]
    [HeaderAuth]
    public async Task<IActionResult> GetUserGames(string username)
    {
        List<GameDataDTO> games = await DataContext.GameData
            .Where(g => g.Player1 == username || g.Player2 == username).ToListAsync();

        return Ok(new GameListDTO(games));
    }

    [HttpGet("{username}/stats")]
    public async Task<IActionResult> GetUserStats(string username)
    {
        if (!await UserDataContext.UserExists(username)) return NotFound();

        var userData = await DataContext.GetUserData(username);
        return Ok(userData.Stats);
    }
}