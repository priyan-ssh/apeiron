using Apeiron.Application.Contracts.Users;
using Apeiron.Application.Interfaces.Users;
using Microsoft.AspNetCore.Mvc;

namespace Apeiron.Api.Controllers.Users;

public class UsersController : BaseApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> Get(Guid id)
    {
        var result = await _userService.GetByIdAsync(id);

        return result.IsSuccess 
            ? Ok(result.Value) 
            : NotFound(result.Error);
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult<UserResponse>> GetByEmail(string email)
    {
        var result = await _userService.GetByEmailAsync(email);

        return result.IsSuccess 
            ? Ok(result.Value) 
            : NotFound(result.Error);
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register(UserRegisterRequest request)
    {
        var result = await _userService.RegisterUserAsync(request);

        return result.IsSuccess 
            ? Ok(result.Value) 
            : BadRequest(result.Error);
    }
}
