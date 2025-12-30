using Microsoft.AspNetCore.Mvc;

namespace Apeiron.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
}
