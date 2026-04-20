using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using NatrixServices.Users;

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
        var userData = await DataContext.GetUserData(username);
        return Ok(userData.LastRequest);
    }

    [HttpGet("requests/count")]
    [HeaderAuth]
    public async Task<IActionResult> GetRequestCount([FromRoute] string username)
    {
        var userData = await DataContext.GetUserData(username);
        return Ok(userData.DnsRequestCount);
    }

    [HttpGet("blocking-state")]
    [HeaderAuth]
    public async Task<IActionResult> GetBlockingState(string username)
    {
        var userData = await DataContext.GetUserData(username);
        return Ok(new BlockingStateDTO { Enabled = userData.EnableBlocking });
    }

    [HttpPatch("blocking-state")]
    [HeaderAuth]
    public async Task<IActionResult> PatchBlockingState(string username, [FromBody] BlockingStateDTO state)
    {
        var userData = await DataContext.GetUserData(username);
        userData.EnableBlocking = state.Enabled;
        await DataContext.SaveChangesAsync();
        return Ok();
    }
}

[Route("api/dnsblocker/users/{username}/devices")]
[HeaderAuth]
public class UserDeviceAPI(DataContext DataContext) : ListAPI<DeviceConfigDTO>("api/dnsblocker/users/{username}/devices")
{
    protected string Username => RouteData.Values["username"]!.ToString()!;
    protected override async Task<Dictionary<string, DeviceConfigDTO>?> GetData() => (await DataContext.GetUserData(Username)).Devices;
    protected override async Task SaveChanges()
    {
        var entry = DataContext.ChangeTracker.Entries<UserData>().FirstOrDefault(e => e.Entity.Username == Username);
        entry?.Property(u => u.Devices).IsModified = true;
        await DataContext.SaveChangesAsync();
    }

    protected override object? GetItemProperty(string property, DeviceConfigDTO obj, out string? error)
    {
        error = null;
        if (property == "blocking-state")
            return new BlockingStateDTO { Enabled = obj.EnableBlocking };

        error = "Invalid property";
        return null;
    }

    protected override string? PatchItemProperty(string property, DeviceConfigDTO obj, JsonElement jsonData)
    {
        if (property == "blocking-state")
        {
            if (!TryAs<BlockingStateDTO>(jsonData, out var blockingState))
                return "Expected blocking state";
            obj.EnableBlocking = blockingState.Enabled;
            return null;
        }
        return "Invalid property";
    }
}

[Route("api/dnsblocker/users/{username}/filters")]
[HeaderAuth]
public class UserFilterAPI(DataContext DataContext) : ListAPI<FilterReferenceDTO>("api/dnsblocker/users/{username}/filters")
{
    protected string Username => RouteData.Values["username"]!.ToString()!;
    protected override async Task<Dictionary<string, FilterReferenceDTO>?> GetData() => (await DataContext.GetUserData(Username)).Filters;
    protected override async Task SaveChanges()
    {
        var entry = DataContext.ChangeTracker.Entries<UserData>().FirstOrDefault(e => e.Entity.Username == Username);
        entry?.Property(u => u.Filters).IsModified = true;
        await DataContext.SaveChangesAsync();
    }

    protected override object? GetItemProperty(string property, FilterReferenceDTO obj, out string? error)
    {
        error = null;
        if (property == "blocking-state")
            return new BlockingStateDTO { Enabled = obj.EnableBlocking };

        error = "Invalid property";
        return null;
    }

    protected override string? PatchItemProperty(string property, FilterReferenceDTO obj, JsonElement jsonData)
    {
        if (property == "blocking-state")
        {
            if (!TryAs<BlockingStateDTO>(jsonData, out var blockingState))
                return "Expected blocking state";
            obj.EnableBlocking = blockingState.Enabled;
            return null;
        }
        return "Invalid property";
    }
}