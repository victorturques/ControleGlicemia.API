using ControleGlicemia.API.DTOs.User;
using ControleGlicemia.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace ControleGlicemia.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var user = await _authService.RegisterAsync(registerDto);

        if (user is null)
            return BadRequest(new { message = "Email já cadastrado." });

        var token = _authService.GenerateJwtToken(user);
        return Ok(new { Token = token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var token = await _authService.LoginAsync(loginDto);

        if (token is null)
            return Unauthorized(new { message = "Credenciais inválidas." });

        return Ok(new { Token = token });
    }
}