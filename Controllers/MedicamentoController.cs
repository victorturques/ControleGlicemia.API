using ControleGlicemia.API.DTOs.Medicamento;
using ControleGlicemia.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ControleGlicemia.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MedicamentoController : ControllerBase
{
    private readonly IMedicamentoService _medicamentoService;

    public MedicamentoController(IMedicamentoService medicamentoService)
    {
        _medicamentoService = medicamentoService;
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
        var medicamentos = await _medicamentoService.GetAllMedicamentosByUserIdAsync(userId);
        return Ok(medicamentos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetUserIdFromClaims();
        var medicamento = await _medicamentoService.GetMedicamentoByIdAsync(id);

        if (medicamento == null || medicamento.UserId != userId)
            return NotFound("Medicamento não encontrado ou você não tem permissão para acessá-lo.");

        return Ok(medicamento);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateMedicamentoDto medicamentoDto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var userId = GetUserIdFromClaims();
        await _medicamentoService.AddMedicamentoAsync(userId, medicamentoDto);

        // Sem id fake por enquanto
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMedicamentoDto medicamentoDto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (id != medicamentoDto.Id)
            return BadRequest("O ID da rota não corresponde ao ID do medicamento fornecido.");

        var userId = GetUserIdFromClaims();
        var updatedMedicamento = await _medicamentoService.UpdateMedicamentoAsync(id, userId, medicamentoDto);

        if (updatedMedicamento == null)
            return NotFound(new { message = "Medicamento não encontrado ou você não tem permissão para atualizá-lo." });

        return Ok(updatedMedicamento);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserIdFromClaims();
        var medicamento = await _medicamentoService.GetMedicamentoByIdAsync(id);

        if (medicamento == null || medicamento.UserId != userId)
            return NotFound("Medicamento não encontrado ou você não tem permissão para excluí-lo.");

        await _medicamentoService.DeleteMedicamentoAsync(id);
        return NoContent();
    }
}