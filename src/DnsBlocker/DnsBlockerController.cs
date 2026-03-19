using Microsoft.AspNetCore.Mvc;

namespace NatrixServices;

[ApiController]
[Route("api/dnsBlocker")]
public class DnsBlockerController : ControllerBase
{

    [HttpGet("{userId:alpha}")]
    public IActionResult GetDnsConfig(string userId)
    {
        var user = NatrixUser.GetUser(userId);

        if (!ConfigHandler.TryLoadUserConfig<DnsBlockerConfig>(user, out var config))
        {
            config = new(true, [], []);
        }

        return Ok(config);
    }

    [HttpPost("{userId:alpha}")]
    public IActionResult SetDnsConfig(string userId, [FromBody] DnsBlockerConfig config)
    {
        var user = NatrixUser.GetUser(userId);

        ConfigHandler.SetUserConfig(user, config);

        return Ok();
    }
}