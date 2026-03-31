using Microsoft.AspNetCore.Mvc;

namespace NatrixServices.DnsBlocker;

[Route("api/dnsblocker")]
[ApiController]
public class API(DataContext DataContext, ConfigContext ConfigContext) : ControllerBase
{
    [HttpPost("createuser")]
    public async Task<IActionResult> CreateUser()
    {
        UserId userId = Utility.GenerateId();

        await DataContext.UserData.AddAsync(new UserData() { UserId = userId });
        await DataContext.SaveChangesAsync();

        await ConfigContext.UserData.AddAsync(new UserConfig() { UserId = userId });
        await ConfigContext.SaveChangesAsync();

        return Created("", userId);
    }
}