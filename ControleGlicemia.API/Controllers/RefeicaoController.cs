using ControleGlicemia.API.Extensions;
using ControleGlicemia.API.DTOs.Refeicao;
using ControleGlicemia.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleGlicemia.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RefeicaoController : ControllerBase
{
    private readonly IRefeicaoService _refeicaoService;
    private readonly ILogger<RefeicaoController> _logger;

    public RefeicaoController(IRefeicaoService refeicaoService, ILogger<RefeicaoController> logger)
    {
        _refeicaoService = refeicaoService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRefeicoes([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = User.GetUserId();
        var refeicoes = await _refeicaoService.GetRefeicoesPagedAsync(userId, page, pageSize);
        return Ok(ApiResponse<object>.Ok(refeicoes));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRefeicaoById(int id)
    {
        var userId = User.GetUserId();
        var refeicao = await _refeicaoService.GetRefeicaoByIdAsync(id, userId);

        if (refeicao == null)
            return NotFound(ApiResponse<object>.NotFound("Refeição não encontrada."));

        return Ok(ApiResponse<object>.Ok(refeicao));
    }

    [HttpPost]
    public async Task<IActionResult> AddRefeicao([FromBody] CreateRefeicaoDto refeicaoDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Dados inválidos.", ModelState.Values
                .SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));

        var userId = User.GetUserId();
        await _refeicaoService.AddRefeicaoAsync(userId, refeicaoDto);

        return CreatedAtAction(null, ApiResponse<object>.Created(new { }));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRefeicao(int id, [FromBody] UpdateRefeicaoDto refeicaoDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Dados inválidos.", ModelState.Values
                .SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));

        if (id != refeicaoDto.Id)
            return BadRequest(ApiResponse<object>.Fail("O ID da rota não corresponde ao ID da refeição fornecida."));

        var userId = User.GetUserId();
        var updatedRefeicao = await _refeicaoService.UpdateRefeicaoAsync(id, userId, refeicaoDto);

        if (updatedRefeicao == null)
            return NotFound(ApiResponse<object>.NotFound("Refeição não encontrada."));

        return Ok(ApiResponse<object>.Ok(updatedRefeicao, "Refeição atualizada com sucesso."));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRefeicao(int id)
    {
        var userId = User.GetUserId();
        var deleted = await _refeicaoService.DeleteRefeicaoAsync(id, userId);

        if (!deleted)
            return NotFound(ApiResponse<object>.NotFound("Refeição não encontrada."));

        return Ok(ApiResponse<object>.Ok(new { }, "Refeição excluída com sucesso."));
    }
}