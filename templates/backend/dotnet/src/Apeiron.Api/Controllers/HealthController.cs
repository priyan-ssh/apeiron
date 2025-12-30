using Microsoft.AspNetCore.Mvc;

namespace Apeiron.Api.Controllers;

public class HealthController : BaseApiController
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
    }
}
