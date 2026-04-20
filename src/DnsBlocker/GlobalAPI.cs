using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NatrixServices.Users;

namespace NatrixServices.DnsBlocker;

[Route("api/dnsblocker/global")]
[ApiController]
public class GlobalAPI(DataContext DataContext) : ControllerBase
{
    [HttpGet("requests/last")]
    [AdminOnly]
    public async Task<IActionResult> GetLastRequest()
    {
        GlobalData globalData = await DataContext.GetGlobalData();

        return Ok(globalData.LastRequest);
    }

    [HttpGet("requests/count")]
    [AdminOnly]
    public async Task<IActionResult> GetRequestCount()
    {
        GlobalData globalData = await DataContext.GetGlobalData();

        return Ok(globalData.DnsRequestCount);
    }

    [HttpGet("blocking-state")]
    [HeaderAuth]
    public async Task<IActionResult> GetBlockingState()
    {
        var data = await DataContext.GetGlobalData();
        return Ok(new BlockingStateDTO { Enabled = data.EnableBlocking });
    }

    [HttpPatch("blocking-state")]
    [HeaderAuth]
    public async Task<IActionResult> PatchBlockingState([FromBody] BlockingStateDTO state)
    {
        var data = await DataContext.GetGlobalData();
        data.EnableBlocking = state.Enabled;
        await DataContext.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("dns-state")]
    [HeaderAuth]
    public async Task<IActionResult> GetDnsState()
    {
        var data = await DataContext.GetGlobalData();
        return Ok(new DnsStateDTO { Enabled = data.EnableDnsServer });
    }

    [HttpPatch("dns-state")]
    [HeaderAuth]
    public async Task<IActionResult> PatchDnsState([FromBody] DnsStateDTO state)
    {
        var data = await DataContext.GetGlobalData();
        data.EnableDnsServer = state.Enabled;
        await DataContext.SaveChangesAsync();
        return Ok();
    }
}

[Route("api/dnsblocker/global/filters")]
[AdminOnly]
public class GlobalFilterAPI(DataContext DataContext) : ListAPI<FilterConfigDTO>("api/dnsblocker/global/filters")
{
    [AllowAnonymous]
    [HttpGet]
    public override async Task<IActionResult> GetAllItems() => await base.GetAllItems();

    protected override async Task<Dictionary<string, FilterConfigDTO>?> GetData() => (await DataContext.GetGlobalData()).Filters;
    protected override async Task SaveChanges()
    {
        var entry = DataContext.ChangeTracker.Entries<GlobalData>().FirstOrDefault();
        entry?.Property(u => u.Filters).IsModified = true;
        await DataContext.SaveChangesAsync();
    }

    protected override object? GetItemProperty(string property, FilterConfigDTO obj, out string? error)
    {
        error = "Invalid property";
        return null;
    }

    protected override string? PatchItemProperty(string property, FilterConfigDTO obj, JsonElement newData) => "Invalid property";
}