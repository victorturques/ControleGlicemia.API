using ControleGlicemia.API.DTOs.RegistroDiario;
using ControleGlicemia.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ControleGlicemia.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RegistroDiarioController : ControllerBase
{
    private readonly IRegistroDiarioService _registroDiarioService;

    public RegistroDiarioController(IRegistroDiarioService registroDiarioService)
    {
        _registroDiarioService = registroDiarioService;
    }

    private int GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            throw new UnauthorizedAccessException("User ID not found or invalid in token.");

        return userId;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllByUserId()
    {
        var userId = GetUserIdFromClaims();
        var registrosDiarios = await _registroDiarioService.GetAllRegistrosDiariosByUserIdAsync(userId);
        return Ok(registrosDiarios);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetUserIdFromClaims();
        var registroDiario = await _registroDiarioService.GetRegistroDiarioByIdAsync(id);

        if (registroDiario == null || registroDiario.UserId != userId)
            return NotFound("Registro diário não encontrado ou você não tem permissão para acessá-lo.");

        return Ok(registroDiario);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateRegistroDiarioDto registroDiarioDto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var userId = GetUserIdFromClaims();
        await _registroDiarioService.AddRegistroDiarioAsync(userId, registroDiarioDto);

        // Sem id fake por enquanto
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRegistroDiarioDto registroDiarioDto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (id != registroDiarioDto.Id)
            return BadRequest("O ID da rota não corresponde ao ID do registro diário fornecido.");

        var userId = GetUserIdFromClaims();
        var updatedRegistroDiario = await _registroDiarioService.UpdateRegistroDiarioAsync(id, userId, registroDiarioDto);

        if (updatedRegistroDiario == null)
            return NotFound(new { message = "Registro diário não encontrado ou você não tem permissão para atualizá-lo." });

        return Ok(updatedRegistroDiario);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserIdFromClaims();
        var registroDiario = await _registroDiarioService.GetRegistroDiarioByIdAsync(id);

        if (registroDiario == null || registroDiario.UserId != userId)
            return NotFound("Registro diário não encontrado ou você não tem permissão para excluí-lo.");

        await _registroDiarioService.DeleteRegistroDiarioAsync(id);
        return NoContent();
    }
}