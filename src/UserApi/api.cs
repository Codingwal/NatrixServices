using Microsoft.AspNetCore.Mvc;

namespace NatrixServices;

[ApiController]
[Route("api/user")]
public class UserAPI : ControllerBase
{

    [HttpGet("create/{username:alpha}")]
    public IActionResult CreateUser(string username)
    {
        var user = UserManager.CreateUser(username);
        return Ok(user);
    }

    [HttpGet("all")]
    public IActionResult GetAllUsers(string userId, [FromQuery] string password)
    {

    }
}