using Microsoft.AspNetCore.Mvc;

namespace NatrixServices;

[ApiController]
[Route("api/dnsBlocker")]
public class DnsBlockerAPI(DataContext dataContext) : ControllerBase
{
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetDnsConfigAsync(UserId userId)
    {
        DnsBlockerConfig? config = await dataContext.DnsBlockerConfigs.FindAsync(userId);

        if (config == null)
            return NotFound($"No dns config for user id \"{userId}\"");

        return Ok(config);
    }

    [HttpPost("{userId}")]
    public async Task<IActionResult> SetDnsConfigAsync(UserId userId, [FromBody] DnsBlockerConfig config)
    {
        if (userId != config.UserId)
            return BadRequest($"User id \"{userId}\" in url doesn't match user id \"{config.UserId}\" in body");

        await dataContext.AddOrUpdate(dataContext.DnsBlockerConfigs, userId, config);
        await dataContext.SaveChangesAsync();

        return Ok();
    }
}