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
        UserData? userData = await DataContext.GetUserData(username);

        if (userData == null)
            return NotFound("User not found");

        return Ok(new UserDataDTO(userData));
    }

    [HttpGet("{username}/games")]
    [HeaderAuth]
    public async Task<IActionResult> GetUserGames(string username, [FromQuery] bool onlyPublic = true)
    {
        if (!onlyPublic && !Auth.AuthenticateUser(HttpContext.Request, UserDataContext))
            return Unauthorized("You can only view private games if you are authenticated");

        List<GameData> games = await DataContext.GameData
            .Where(g => (!onlyPublic || g.IsPublic) && (g.Player1 == username || g.Player2 == username)).ToListAsync();

        return Ok(new GameListDTO { Games = games });
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