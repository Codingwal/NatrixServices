using Microsoft.AspNetCore.Mvc;

namespace NatrixServices.DnsBlocker;

[Route("api/dnsblocker/config")]
public class ConfigUserAPI(ConfigContext ConfigContext) : ControllerBase
{
    [HttpGet("blockingenabled/{userId}")]
    public async Task<IActionResult> GetBlockingEnabled(UserId userId, [FromQuery] DeviceId? deviceId)
    {
        IBlockingConfig? config = await GetBlockingConfig(userId, deviceId);
        if (config == null) return NotFound();

        return Ok(config.EnableBlocking);
    }

    [HttpPatch("blockingenabled/{userId}")]
    public async Task<IActionResult> SetBlockingEnabled(UserId userId, [FromQuery] bool enabled, [FromQuery] DeviceId? deviceId)
    {
        IBlockingConfig? config = await GetBlockingConfig(userId, deviceId);
        if (config == null) return NotFound();

        config.EnableBlocking = enabled;
        await ConfigContext.SaveChangesAsync();
        return Ok();
    }


    [HttpGet("devices/{userId}")]
    public async Task<IActionResult> GetDevices(UserId userId)
    {
        UserConfig? userConfig = await ConfigContext.GetUserData(userId);
        if (userConfig == null) return NotFound();

        return Ok(userConfig.Devices);
    }

    [HttpPatch("devices/{userId}")]
    public async Task<IActionResult> SetDevices(UserId userId, [FromBody] List<DeviceConfig> devices)
    {
        UserConfig? userConfig = await ConfigContext.GetUserData(userId);
        if (userConfig == null) return NotFound();

        userConfig.Devices = devices;

        return Ok();
    }


    [HttpGet("filters/{userId}")]
    public async Task<IActionResult> GetFilters(UserId userId)
    {
        UserConfig? userConfig = await ConfigContext.GetUserData(userId);
        if (userConfig == null) return NotFound();

        return Ok(userConfig.Filters);
    }

    [HttpPatch("filters/{userId}")]
    public async Task<IActionResult> SetFilters(UserId userId, [FromBody] List<FilterReference> filters)
    {
        UserConfig? userConfig = await ConfigContext.GetUserData(userId);
        if (userConfig == null) return NotFound();

        userConfig.Filters = filters;

        return Ok();
    }


    private async Task<IBlockingConfig?> GetBlockingConfig(UserId userId, DeviceId? deviceId)
    {
        UserConfig? userConfig = await ConfigContext.GetUserData(userId);
        if (userConfig == null) return null;

        if (deviceId == null)
            return userConfig;

        DeviceConfig? deviceConfig = userConfig.Devices.FirstOrDefault(device => device.Device == deviceId);

        return deviceConfig;
    }
}

[Route("api/dnsblocker/config/global")]
public class ConfigGlobalAPI(ConfigContext ConfigContext) : ControllerBase
{
    [HttpGet("blockingenabled")]
    public async Task<IActionResult> GetBlockingEnabled()
    {
        GlobalConfig config = await ConfigContext.GetGlobalData();

        return Ok(config.EnableBlocking);
    }

    [HttpPatch("blockingenabled")]
    [AdminOnly]
    public async Task<IActionResult> SetBlockingEnabled([FromQuery] bool enabled)
    {
        GlobalConfig config = await ConfigContext.GetGlobalData();

        config.EnableBlocking = enabled;
        await ConfigContext.SaveChangesAsync();

        return Ok();
    }


    [HttpGet("dnsenabled")]
    public async Task<IActionResult> GetDnsEnabled()
    {
        GlobalConfig config = await ConfigContext.GetGlobalData();

        return Ok(config.EnableDnsServer);
    }

    [HttpPatch("dnsenabled")]
    [AdminOnly]
    public async Task<IActionResult> SetDnsEnabled([FromQuery] bool enabled)
    {
        GlobalConfig config = await ConfigContext.GetGlobalData();

        config.EnableDnsServer = enabled;
        await ConfigContext.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("filters")]
    public async Task<IActionResult> GetFilters()
    {
        GlobalConfig config = await ConfigContext.GetGlobalData();

        return Ok(config.Filters);
    }

    [HttpPatch("filters/add")]
    [AdminOnly]
    public async Task<IActionResult> AddFilter([FromBody] FilterConfig filter)
    {
        GlobalConfig config = await ConfigContext.GetGlobalData();

        config.Filters.Add(filter.Id, filter);
        await ConfigContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPatch("filters/remove")]
    [AdminOnly]
    public async Task<IActionResult> AddFilter([FromQuery] FilterId filterId)
    {
        GlobalConfig config = await ConfigContext.GetGlobalData();

        config.Filters.Remove(filterId);
        await ConfigContext.SaveChangesAsync();

        return Ok();
    }
}