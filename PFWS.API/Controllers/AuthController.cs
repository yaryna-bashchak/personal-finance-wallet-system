using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PFWS.API.Controllers.Controllers;
using PFWS.BusinessLogicLayer.DTOs.Auth;
using PFWS.BusinessLogicLayer.Services;

namespace PFWS.API.Controllers;

public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var result = await _authService.Register(registerDto);

        if (result.Succeeded)
        {
            return Ok("User registered successfully");
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        try
        {
            var token = await _authService.Login(loginDto);
            return Ok(token);
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("currentUser")]
    public async Task<ActionResult<LoginedUserDto>> GetCurrentUser()
    {
        try
        {
            var loginedUser = await _authService.GetCurrentUser(User.Identity.Name);
            return loginedUser;
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}
