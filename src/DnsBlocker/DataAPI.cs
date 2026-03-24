using Microsoft.AspNetCore.Mvc;

namespace NatrixServices.DnsBlocker;

[Route("api/dnsblocker/data")]
public class DataAPI(DataContext DataContext) : ControllerBase
{
    [HttpGet("lastrequest/{userId}")]
    public async Task<IActionResult> GetLastRequest(UserId userId)
    {
        UserData? userData = await DataContext.GetUserData(userId);

        if (userData == null)
            return NotFound();

        return Ok(userData.LastRequest);
    }

    [HttpGet("dnsrequestcount/{userId}")]
    public async Task<IActionResult> GetRequestCount(UserId userId)
    {
        UserData? userData = await DataContext.GetUserData(userId);

        if (userData == null)
            return NotFound();

        return Ok(userData.DnsRequestCount);
    }
}