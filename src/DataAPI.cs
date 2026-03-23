using Microsoft.AspNetCore.Mvc;

namespace NatrixServices;

[ApiController]
public class DataAPI<TUser, TGlobal>(DataContext<TUser, TGlobal> DataContext, bool AdminOnlySet) : ControllerBase
    where TUser : UserDataBase
    where TGlobal : class, new()
{
    [HttpGet("global")]
    [AdminOnly]
    public async Task<IActionResult> GetGlobalData()
    {
        TGlobal globalData = await DataContext.GetGlobalData();
        return Ok(globalData);
    }

    [HttpPost("global")]
    [AdminOnly]
    public async Task<IActionResult> SetGlobalData([FromBody] TGlobal data)
    {
        await DataContext.SetGlobalData(data);
        return Ok();
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetData(UserId userId)
    {
        TUser? userData = await DataContext.GetUserData(userId);

        if (userData == null)
            return NotFound($"Could not find data for user with id \"{userId}\"");

        return Ok(userData);
    }

    [HttpPost("")]
    public async Task<IActionResult> SetData([FromBody] TUser data)
    {
        if (AdminOnlySet)
        {
            string? error = Auth.VerifyPassword(Request);

            if (error != null)
                return Unauthorized(error);
        }

        await DataContext.SetUserData(data.UserId, data);

        return Ok();
    }
}