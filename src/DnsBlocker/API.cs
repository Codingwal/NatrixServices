using Microsoft.AspNetCore.Mvc;

namespace NatrixServices.DnsBlocker;

[Route("api/dnsblocker")]
[ApiController]
public class API(DataContext DataContext, ConfigContext ConfigContext) : ControllerBase
{
    [HttpPost("createuser")]
    public async Task<IActionResult> CreateUser()
    {
        UserId userId = NatrixServices.User.Create();

        await DataContext.SetUserData(userId, new UserData() { UserId = userId });
        await ConfigContext.SetUserData(userId, new UserConfig() { UserId = userId });

        Console.WriteLine($"Created user {userId}");

        return Created("", userId);
    }
}