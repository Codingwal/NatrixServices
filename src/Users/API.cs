using Microsoft.AspNetCore.Mvc;
using NatrixServices.Users;

[ApiController]
[Route("api/users")]
public class Api(DataContext DataContext) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        UserData userData = new() { Username = request.Username, PasswordHash = request.PasswordHash };
        await DataContext.UserData.AddAsync(userData);
        await DataContext.SaveChangesAsync();
        return Created($"api/users/{userData.Username}", userData.Username);
    }
    public record CreateUserRequest(string Username, string PasswordHash);
}