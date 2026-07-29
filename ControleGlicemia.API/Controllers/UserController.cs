using System.IdentityModel.Tokens.Jwt;
using ControleGlicemia.API.DTOs.User;
using ControleGlicemia.API.Extensions;
using ControleGlicemia.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleGlicemia.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, IAuthService authService, ILogger<UserController> logger)
    {
        _userService = userService;
        _authService = authService;
        _logger = logger;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.GetUserId();
        var profile = await _userService.GetProfileAsync(userId);

        if (profile is null)
            return NotFound(ApiResponse<object>.NotFound("Usuário não encontrado."));

        return Ok(ApiResponse<object>.Ok(profile));
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Dados inválidos.", ModelState.Values
                .SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));

        var userId = User.GetUserId();
        var updatedProfile = await _userService.UpdateProfileAsync(userId, updateDto);

        if (updatedProfile is null)
            return NotFound(ApiResponse<object>.NotFound("Usuário não encontrado."));

        return Ok(ApiResponse<object>.Ok(updatedProfile, "Perfil atualizado com sucesso."));
    }

    [HttpDelete("account")]
    public async Task<IActionResult> DeleteAccount()
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

        var result = await _userService.DeleteAccountAsync(userId);

        if (!result)
            return NotFound(ApiResponse<object>.NotFound("Usuário não encontrado."));

        return Ok(ApiResponse<object>.Ok(new { }, "Conta excluída com sucesso."));
    }
}
