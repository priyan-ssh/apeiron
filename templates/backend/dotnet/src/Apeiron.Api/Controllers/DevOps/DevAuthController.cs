#if DEBUG
using Apeiron.Application.Common.Models;
using Apeiron.Application.Interfaces.Auth;
using Apeiron.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace Apeiron.Api.Controllers.DevOps;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/dev")]
public class DevAuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public DevAuthController(UserManager<User> userManager, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    /// <summary>
    /// [DEBUG ONLY] Generates a magic token for the given email without password check.
    /// </summary>
    [HttpPost("token")]
    public async Task<ActionResult<Result<string>>> GetMagicToken([FromBody] string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return NotFound(Result.Failure(new Error("User.NotFound", $"No user found with email {email}")));
        }

        var token = _jwtTokenGenerator.GenerateToken(user);
        return Ok(Result<string>.Success(token));
    }
}
#endif
