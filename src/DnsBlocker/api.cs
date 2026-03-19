using Microsoft.AspNetCore.Mvc;

namespace NatrixServices;

[ApiController]
[Route("api/dnsBlocker")]
public class DnsBlockerAPI : ControllerBase
{

    [HttpGet("{userId:alpha}")]
    public IActionResult GetDnsConfig(string userId)
    {
        var user = UserManager.GetUser(userId);

        if (!DataHandler.TryLoadUserData<DnsBlockerConfig>(user, out var config))
        {
            config = new(true, [], []);
        }

        return Ok(config);
    }

    [HttpPost("{userId:alpha}")]
    public IActionResult SetDnsConfig(string userId, [FromBody] DnsBlockerConfig config)
    {
        var user = UserManager.GetUser(userId);

        DataHandler.SetUserData(user, config);

        return Ok();
    }
}