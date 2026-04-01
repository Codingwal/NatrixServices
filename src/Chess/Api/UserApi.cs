using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NatrixServices.Chess;

[ApiController]
[Route("api/chess/users")]
public class UserApi(DataContext DataContext) : ControllerBase
{
    [HttpGet("{username}")]
    public async Task<IActionResult> GetUserData(string username)
    {
        UserData? userData = await DataContext.GetUserData(username);

        if (userData == null)
            return NotFound("User not found");

        return Ok(new UserDataDTO(userData));
    }

    [HttpGet("{username}/games")]
    [HeaderAuth]
    public async Task<IActionResult> GetUserGames(string username)
    {
        List<GameData> games = await DataContext.GameData
            .Where(g => g.Player1 == username || g.Player2 == username).ToListAsync();

        return Ok(new GameListDTO(games));
    }

    [HttpGet("{username}/stats")]
    public async Task<IActionResult> GetUserStats(string username)
    {
        UserData? userData = await DataContext.GetUserData(username);

        if (userData == null)
            return NotFound("User not found");

        return Ok(userData.Stats);
    }
}