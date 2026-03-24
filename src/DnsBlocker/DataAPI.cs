using Microsoft.AspNetCore.Mvc;

namespace NatrixServices.DnsBlocker;

[Route("api/dnsblocker/data")]
[ApiController]
public class DataUserAPI(DataContext DataContext) : ControllerBase
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

[Route("api/dnsblocker/data/global")]
[ApiController]
public class DataGlobalAPI(DataContext DataContext) : ControllerBase
{
    [HttpGet("lastrequest")]
    [AdminOnly]
    public async Task<IActionResult> GetLastRequest()
    {
        GlobalData globalData = await DataContext.GetGlobalData();

        return Ok(globalData.LastRequest);
    }

    [HttpGet("dnsrequestcount")]
    [AdminOnly]
    public async Task<IActionResult> GetRequestCount()
    {
        GlobalData globalData = await DataContext.GetGlobalData();

        return Ok(globalData.DnsRequestCount);
    }
}