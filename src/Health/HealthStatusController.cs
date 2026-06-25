using Microsoft.AspNetCore.Mvc;
using NatrixServices.Shared.Application;

namespace NatrixServices.Health;

[ApiController]
public class HealthStatusController : ControllerBase
{
    [HttpGet]
    [Route("api/health")]
    [NoAuth]
    public async Task<IActionResult> GetHealthStatus()
    {
        return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
    }
}