using Microsoft.AspNetCore.Mvc;

namespace NatrixServices.DnsBlocker;

[Route("api/dnsblocker/users/{username}/")]
[ApiController]
public class UserAPI(DataContext DataContext) : ControllerBase
{
    /* User data */

    [HttpGet("requests/last")]
    [HeaderAuth]
    public async Task<IActionResult> GetLastRequest([FromRoute] string username)
    {
        UserData? userData = await DataContext.GetUserData(username);
        if (userData == null) return NotFound();
        return Ok(userData.LastRequest);
    }

    [HttpGet("requests/count")]
    [HeaderAuth]
    public async Task<IActionResult> GetRequestCount([FromRoute] string username)
    {
        UserData? userData = await DataContext.GetUserData(username);
        if (userData == null) return NotFound();
        return Ok(userData.DnsRequestCount);
    }

    [HttpGet("blocking-state")]
    [HeaderAuth]
    public async Task<IActionResult> GetBlockingState(string username)
    {
        UserData? userData = await DataContext.GetUserData(username);
        if (userData == null) return NotFound();
        return Ok(new BlockingStateDTO { Enabled = userData.EnableBlocking });
    }

    [HttpPatch("blocking-state")]
    [HeaderAuth]
    public async Task<IActionResult> PatchBlockingState(string username, [FromBody] BlockingStateDTO state)
    {
        UserData? userData = await DataContext.GetUserData(username);
        if (userData == null) return NotFound();
        userData.EnableBlocking = state.Enabled;
        await DataContext.SaveChangesAsync();
        return Ok();
    }
}

[Route("api/dnsblocker/users/{username}/devices")]
[HeaderAuth]
public class UserDeviceAPI(DataContext DataContext) : ListAPI<DeviceConfigDTO>("api/dnsblocker/users/{username}/devices")
{
    protected string Username => RouteData.Values["username"]?.ToString()!;
    protected override async Task<Dictionary<string, DeviceConfigDTO>?> GetData()
    {
        var userData = await DataContext.GetUserData(Username);
        if (userData == null) return null;
        return userData.Devices;
    }
    protected override async Task SaveChanges() => await DataContext.SaveChangesAsync();

    protected override object? GetItemProperty(string property, DeviceConfigDTO obj, out string? error)
    {
        error = null;
        if (property == "blocking-state")
            return new BlockingStateDTO { Enabled = obj.EnableBlocking };

        error = "Invalid property";
        return null;
    }

    protected override string? PatchItemProperty(string property, DeviceConfigDTO obj, object newData)
    {
        if (property == "blocking-state")
        {
            if (newData is not BlockingStateDTO blockingState)
                return "Expected blocking state";
            obj.EnableBlocking = blockingState.Enabled;
        }
        return "Invalid property";
    }
}

[Route("api/dnsblocker/users/{username}/filters")]
[HeaderAuth]
public class UserFilterAPI(DataContext DataContext) : ListAPI<FilterReferenceDTO>("api/dnsblocker/users/{username}/filters")
{
    protected string Username => RouteData.Values["username"]?.ToString()!;
    protected override async Task<Dictionary<string, FilterReferenceDTO>?> GetData()
    {
        var userData = await DataContext.GetUserData(Username);
        if (userData == null) return null;
        return userData.Filters;
    }
    protected override async Task SaveChanges() => await DataContext.SaveChangesAsync();

    protected override object? GetItemProperty(string property, FilterReferenceDTO obj, out string? error)
    {
        error = null;
        if (property == "blocking-state")
            return new BlockingStateDTO { Enabled = obj.EnableBlocking };

        error = "Invalid property";
        return null;
    }

    protected override string? PatchItemProperty(string property, FilterReferenceDTO obj, object newData)
    {
        if (property == "blocking-state")
        {
            if (newData is not BlockingStateDTO blockingState)
                return "Expected blocking state";
            obj.EnableBlocking = blockingState.Enabled;
        }
        return "Invalid property";
    }
}