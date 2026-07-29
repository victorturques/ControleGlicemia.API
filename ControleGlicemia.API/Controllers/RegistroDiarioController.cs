using ControleGlicemia.API.Extensions;
using ControleGlicemia.API.DTOs.RegistroDiario;
using ControleGlicemia.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleGlicemia.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RegistroDiarioController : ControllerBase
{
    private readonly IRegistroDiarioService _registroDiarioService;
    private readonly ILogger<RegistroDiarioController> _logger;

    public RegistroDiarioController(IRegistroDiarioService registroDiarioService, ILogger<RegistroDiarioController> logger)
    {
        _registroDiarioService = registroDiarioService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllByUserId([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = User.GetUserId();
        var registrosDiarios = await _registroDiarioService.GetRegistrosDiariosPagedAsync(userId, page, pageSize);
        return Ok(ApiResponse<object>.Ok(registrosDiarios));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = User.GetUserId();
        var registroDiario = await _registroDiarioService.GetRegistroDiarioByIdAsync(id, userId);

        if (registroDiario == null)
            return NotFound(ApiResponse<object>.NotFound("Registro diário não encontrado."));

        return Ok(ApiResponse<object>.Ok(registroDiario));
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateRegistroDiarioDto registroDiarioDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Dados inválidos.", ModelState.Values
                .SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));

        var userId = User.GetUserId();
        await _registroDiarioService.AddRegistroDiarioAsync(userId, registroDiarioDto);

        return CreatedAtAction(null, ApiResponse<object>.Created(new { }));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRegistroDiarioDto registroDiarioDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Dados inválidos.", ModelState.Values
                .SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));

        if (id != registroDiarioDto.Id)
            return BadRequest(ApiResponse<object>.Fail("O ID da rota não corresponde ao ID do registro diário fornecido."));

        var userId = User.GetUserId();
        var updatedRegistroDiario = await _registroDiarioService.UpdateRegistroDiarioAsync(id, userId, registroDiarioDto);

        if (updatedRegistroDiario == null)
            return NotFound(ApiResponse<object>.NotFound("Registro diário não encontrado."));

        return Ok(ApiResponse<object>.Ok(updatedRegistroDiario, "Registro diário atualizado com sucesso."));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();
        var deleted = await _registroDiarioService.DeleteRegistroDiarioAsync(id, userId);

        if (!deleted)
            return NotFound(ApiResponse<object>.NotFound("Registro diário não encontrado."));

        return Ok(ApiResponse<object>.Ok(new { }, "Registro diário excluído com sucesso."));
    }
}