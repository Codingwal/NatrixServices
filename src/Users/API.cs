using Microsoft.AspNetCore.Mvc;

namespace NatrixServices.Users;

[Route("api/users")]
public class API() : ControllerBase
{
    [HttpGet("create")]
    public async Task<IActionResult> GetLastRequest()
    {
        UserId userId = NatrixServices.User.Create();

        return Ok(userId);
    }
}
