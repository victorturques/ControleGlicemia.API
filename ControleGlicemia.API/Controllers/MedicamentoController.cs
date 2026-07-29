using ControleGlicemia.API.Extensions;
using ControleGlicemia.API.DTOs.Medicamento;
using ControleGlicemia.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleGlicemia.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MedicamentoController : ControllerBase
{
    private readonly IMedicamentoService _medicamentoService;
    private readonly ILogger<MedicamentoController> _logger;

    public MedicamentoController(IMedicamentoService medicamentoService, ILogger<MedicamentoController> logger)
    {
        _medicamentoService = medicamentoService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllByUserId([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = User.GetUserId();
        var medicamentos = await _medicamentoService.GetMedicamentosPagedAsync(userId, page, pageSize);
        return Ok(ApiResponse<object>.Ok(medicamentos));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = User.GetUserId();
        var medicamento = await _medicamentoService.GetMedicamentoByIdAsync(id, userId);

        if (medicamento == null)
            return NotFound(ApiResponse<object>.NotFound("Medicamento não encontrado."));

        return Ok(ApiResponse<object>.Ok(medicamento));
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateMedicamentoDto medicamentoDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Dados inválidos.", ModelState.Values
                .SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));

        var userId = User.GetUserId();
        await _medicamentoService.AddMedicamentoAsync(userId, medicamentoDto);

        return CreatedAtAction(null, ApiResponse<object>.Created(new { }));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMedicamentoDto medicamentoDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Dados inválidos.", ModelState.Values
                .SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));

        if (id != medicamentoDto.Id)
            return BadRequest(ApiResponse<object>.Fail("O ID da rota não corresponde ao ID do medicamento fornecido."));

        var userId = User.GetUserId();
        var updatedMedicamento = await _medicamentoService.UpdateMedicamentoAsync(id, userId, medicamentoDto);

        if (updatedMedicamento == null)
            return NotFound(ApiResponse<object>.NotFound("Medicamento não encontrado."));

        return Ok(ApiResponse<object>.Ok(updatedMedicamento, "Medicamento atualizado com sucesso."));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();
        var deleted = await _medicamentoService.DeleteMedicamentoAsync(id, userId);

        if (!deleted)
            return NotFound(ApiResponse<object>.NotFound("Medicamento não encontrado."));

        return Ok(ApiResponse<object>.Ok(new { }, "Medicamento excluído com sucesso."));
    }
}