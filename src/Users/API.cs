using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NatrixServices.Users;

[ApiController]
[Route("api/users")]
public class Api(DataContext DataContext) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        string username = request.Username.ToLower();

        bool userExists = await DataContext.UserData.AnyAsync(u => u.Username == username);
        if (userExists) return BadRequest("A user with this username already exists");

        UserData userData = new() { Username = username, PasswordHash = request.PasswordHash };
        await DataContext.UserData.AddAsync(userData);
        await DataContext.SaveChangesAsync();
        return Created($"api/users/{userData.Username}", new { username = userData.Username });
    }
    public record CreateUserRequest(string Username, string PasswordHash);

    [HttpGet("{username}")]
    [HeaderAuth]
    public async Task<IActionResult> GetUserInfo(string username)
    {
        if (Request.Headers["username"] != username)
            return BadRequest("Username in url and header does not match");

        return Ok(new { username });
    }
}