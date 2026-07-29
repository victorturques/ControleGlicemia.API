using System.IdentityModel.Tokens.Jwt;
using ControleGlicemia.API.DTOs.User;
using ControleGlicemia.API.Extensions;
using ControleGlicemia.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ControleGlicemia.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("AuthPolicy")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Dados inválidos.", ModelState.Values
                .SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));

        var user = await _authService.RegisterAsync(registerDto);

        if (user is null)
            return Conflict(ApiResponse<object>.Fail("Email já cadastrado."));

        var token = _authService.GenerateJwtToken(user);
        return CreatedAtAction(null, ApiResponse<object>.Ok(new { Token = token }, "Cadastro realizado com sucesso."));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Dados inválidos.", ModelState.Values
                .SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));

        var result = await _authService.LoginAsync(loginDto);

        if (result is null)
            return Unauthorized(ApiResponse<object>.Fail("Credenciais inválidas."));

        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto refreshTokenDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Dados inválidos.", ModelState.Values
                .SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));

        var result = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);

        if (result is null)
            return Unauthorized(ApiResponse<object>.Fail("Refresh token inválido ou expirado."));

        return Ok(ApiResponse<object>.Ok(result));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = User.GetUserId();

        var jtiClaim = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        var expClaim = User.FindFirst("exp")?.Value;

        DateTime expiresAt = DateTime.UtcNow.AddHours(1);
        if (long.TryParse(expClaim, out var expUnix))
        {
            expiresAt = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
        }

        await _authService.LogoutAsync(userId, jtiClaim, expiresAt);

        return Ok(ApiResponse<object>.Ok(new { }, "Logout realizado com sucesso."));
    }
}
