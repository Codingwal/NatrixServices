using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NatrixServices;

[ApiController]
[Route("api/user")]
public class UserAPI(DataContext dataContext) : ControllerBase
{

    [HttpPost("create/{username:alpha}")]
    public async Task<IActionResult> CreateUser(string username)
    {
        UserId userId = await UserManager.CreateUserAsync(username, dataContext);
        return Created($"/api/user/{userId}", userId);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser(UserId userId)
    {
        UserData? userData = await dataContext.UserDatas.FindAsync(userId);

        if (userData == null)
            return NotFound($"Couldn't find user with id \"{userId}\"");

        return Ok(userData);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllUsers([FromHeader] string password)
    {
        if (!Auth.VerifyPassword(password))
            return Unauthorized($"Wrong admin password \"{password}\"");

        List<UserData> userDatas = await dataContext.UserDatas.ToListAsync();

        return Ok(userDatas);
    }
}