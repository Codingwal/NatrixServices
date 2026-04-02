using Microsoft.AspNetCore.Mvc;

namespace NatrixServices.DnsBlocker;

[Route("api/dnsblocker/data/global")]
[ApiController]
public class GlobalAPI(DataContext DataContext) : ControllerBase
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