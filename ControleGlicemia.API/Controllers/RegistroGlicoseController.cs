using ControleGlicemia.API.Extensions;
using ControleGlicemia.API.DTOs.RegistroGlicose;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleGlicemia.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RegistroGlicoseController : ControllerBase
{
    private readonly IRegistroGlicoseService _registroGlicoseService;
    private readonly ILogger<RegistroGlicoseController> _logger;

    public RegistroGlicoseController(IRegistroGlicoseService registroGlicoseService, ILogger<RegistroGlicoseController> logger)
    {
        _registroGlicoseService = registroGlicoseService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRegistrosGlicose([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = User.GetUserId();
        var registros = await _registroGlicoseService.GetRegistrosGlicosePagedAsync(userId, page, pageSize);
        return Ok(ApiResponse<object>.Ok(registros));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRegistroGlicoseById(int id)
    {
        var userId = User.GetUserId();
        var registro = await _registroGlicoseService.GetRegistroGlicoseByIdAsync(id, userId);

        if (registro == null)
            return NotFound(ApiResponse<object>.NotFound("Registro de glicose não encontrado."));

        return Ok(ApiResponse<object>.Ok(registro));
    }

    [HttpPost]
    public async Task<IActionResult> AddRegistroGlicose([FromBody] CreateRegistroGlicoseDto registroGlicoseDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Dados inválidos.", ModelState.Values
                .SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));

        var userId = User.GetUserId();
        await _registroGlicoseService.AddRegistroGlicoseAsync(userId, registroGlicoseDto);

        return CreatedAtAction(null, ApiResponse<object>.Created(new { }));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRegistroGlicose(int id, [FromBody] UpdateRegistroGlicoseDto registroGlicoseDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Dados inválidos.", ModelState.Values
                .SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));

        if (id != registroGlicoseDto.Id)
            return BadRequest(ApiResponse<object>.Fail("O ID da rota não corresponde ao ID do registro fornecido."));

        var userId = User.GetUserId();
        var updatedRegistro = await _registroGlicoseService.UpdateRegistroGlicoseAsync(id, userId, registroGlicoseDto);

        if (updatedRegistro == null)
            return NotFound(ApiResponse<object>.NotFound("Registro de glicose não encontrado."));

        return Ok(ApiResponse<object>.Ok(updatedRegistro, "Registro atualizado com sucesso."));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRegistroGlicose(int id)
    {
        var userId = User.GetUserId();
        var deleted = await _registroGlicoseService.DeleteRegistroGlicoseAsync(id, userId);

        if (!deleted)
            return NotFound(ApiResponse<object>.NotFound("Registro de glicose não encontrado."));

        return Ok(ApiResponse<object>.Ok(new { }, "Registro excluído com sucesso."));
    }
}