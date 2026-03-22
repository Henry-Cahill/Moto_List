using Microsoft.AspNetCore.Mvc;
using Moto_List.API.Services;
using Moto_List.Shared.DTOs;

namespace Moto_List.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (result is null)
            return Unauthorized(new { message = "Invalid username or password" });

        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        if (result is null)
            return Conflict(new { message = "Username or email already exists" });

        return Ok(result);
    }
}
