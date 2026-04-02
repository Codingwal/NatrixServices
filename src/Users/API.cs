using Microsoft.AspNetCore.Mvc;

namespace NatrixServices.Users;

[ApiController]
[Route("api/users")]
public class Api(DataContext DataContext) : ControllerBase
{
    [HttpPost]
    [AdminOnly]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        UserData userData = new() { Username = request.Username, PasswordHash = request.PasswordHash };
        await DataContext.UserData.AddAsync(userData);
        await DataContext.SaveChangesAsync();
        return Created($"api/users/{userData.Username}", userData.Username);
    }
    public record CreateUserRequest(string Username, string PasswordHash);
}