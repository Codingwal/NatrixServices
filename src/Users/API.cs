using Microsoft.AspNetCore.Mvc;

namespace NatrixServices.Users;

[ApiController]
[Route("api/users")]
public class Api(IItemStorage<UserData, string> UserDataStorage) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        string username = request.Username.ToLower();

        bool userExists = await UserDataStorage.ItemExistsAsync(username);
        if (userExists) return BadRequest("A user with this username already exists");

        UserData userData = new() { Username = username, PasswordHash = request.PasswordHash };
        await UserDataStorage.AddItemAsync(userData);
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