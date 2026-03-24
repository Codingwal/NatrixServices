using Microsoft.AspNetCore.Mvc;

namespace NatrixServices.DnsBlocker;

[Route("api/dnsblocker/config")]
[ApiController]
public class ConfigUserAPI(ConfigContext ConfigContext) : ControllerBase
{
    [HttpGet("blockingenabled/{userId}")]
    public async Task<IActionResult> GetBlockingEnabled(UserId userId, [FromQuery] DeviceId? deviceId, [FromQuery] FilterId? filterId)
    {
        IBlockingConfig? config = await GetBlockingConfig(userId, deviceId, filterId);
        if (config == null) return NotFound();

        return Ok(config.EnableBlocking);
    }

    [HttpPatch("blockingenabled/{userId}")]
    public async Task<IActionResult> SetBlockingEnabled(UserId userId, [FromQuery] bool enabled, [FromQuery] DeviceId? deviceId, [FromQuery] FilterId? filterId)
    {
        IBlockingConfig? config = await GetBlockingConfig(userId, deviceId, filterId);
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


    [HttpPatch("devices/add/{userId}")]
    public async Task<IActionResult> AddDevice(UserId userId, [FromBody] AddDeviceRequest data)
    {
        UserConfig? userConfig = await ConfigContext.GetUserData(userId);
        if (userConfig == null) return NotFound();

        userConfig.Devices.Add(data.Device.Id, data.Device);
        await ConfigContext.SaveChangesAsync();

        return Ok();
    }
    public record AddDeviceRequest(DeviceConfig Device);

    [HttpPatch("devices/remove/{userId}")]
    public async Task<IActionResult> RemoveDevice(UserId userId, [FromQuery] DeviceId deviceId)
    {
        UserConfig? userConfig = await ConfigContext.GetUserData(userId);
        if (userConfig == null) return NotFound();

        userConfig.Devices.Remove(deviceId);
        await ConfigContext.SaveChangesAsync();

        return Ok();
    }


    [HttpGet("filters/{userId}")]
    public async Task<IActionResult> GetFilters(UserId userId)
    {
        UserConfig? userConfig = await ConfigContext.GetUserData(userId);
        if (userConfig == null) return NotFound();

        return Ok(userConfig.Filters);
    }

    [HttpPatch("filters/add/{userId}")]
    public async Task<IActionResult> AddFilter(UserId userId, [FromBody] AddFilterRequest data)
    {
        UserConfig? userConfig = await ConfigContext.GetUserData(userId);
        if (userConfig == null) return NotFound();

        userConfig.Filters.Add(data.Filter.Id, data.Filter);
        await ConfigContext.SaveChangesAsync();

        return Ok();
    }
    public record AddFilterRequest(FilterReference Filter);

    [HttpPatch("filters/remove/{userId}")]
    public async Task<IActionResult> RemoveFilter(UserId userId, [FromQuery] FilterId filterId)
    {
        UserConfig? userConfig = await ConfigContext.GetUserData(userId);
        if (userConfig == null) return NotFound();

        userConfig.Filters.Remove(filterId);
        await ConfigContext.SaveChangesAsync();

        return Ok();
    }


    private async Task<IBlockingConfig?> GetBlockingConfig(UserId userId, DeviceId? deviceId, FilterId? filterId)
    {
        // Can't get a device and a filter at the same time
        if (deviceId != null && filterId != null)
            return null;

        UserConfig? userConfig = await ConfigContext.GetUserData(userId);
        if (userConfig == null) return null;

        if (deviceId != null)
            return userConfig.Devices.GetValueOrDefault(deviceId);

        if (filterId != null)
            return userConfig.Filters.GetValueOrDefault(filterId);

        return userConfig;
    }
}

[Route("api/dnsblocker/config/global")]
[ApiController]
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
    public async Task<IActionResult> AddFilter([FromBody] AddFilterRequest data)
    {
        GlobalConfig config = await ConfigContext.GetGlobalData();

        config.Filters.Add(data.Filter.Id, data.Filter);
        await ConfigContext.SaveChangesAsync();

        return Ok();
    }
    public record AddFilterRequest(FilterConfig Filter);

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