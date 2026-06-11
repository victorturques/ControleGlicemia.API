using ControleGlicemia.API.DTOs.Refeicao;
using ControleGlicemia.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ControleGlicemia.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RefeicaoController : ControllerBase
{
    private readonly IRefeicaoService _refeicaoService;

    public RefeicaoController(IRefeicaoService refeicaoService)
    {
        _refeicaoService = refeicaoService;
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
    public async Task<IActionResult> GetAllRefeicoes()
    {
        var userId = GetUserIdFromClaims();
        var refeicoes = await _refeicaoService.GetAllRefeicoesByUserIdAsync(userId);
        return Ok(refeicoes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRefeicaoById(int id)
    {
        var userId = GetUserIdFromClaims();
        var refeicao = await _refeicaoService.GetRefeicaoByIdAsync(id);

        if (refeicao == null || refeicao.UserId != userId)
            return NotFound("Refeição não encontrada ou você não tem permissão para acessá-la.");

        return Ok(refeicao);
    }

    [HttpPost]
    public async Task<IActionResult> AddRefeicao([FromBody] CreateRefeicaoDto refeicaoDto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var userId = GetUserIdFromClaims();
        await _refeicaoService.AddRefeicaoAsync(userId, refeicaoDto);

        // Sem id fake por enquanto
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRefeicao(int id, [FromBody] UpdateRefeicaoDto refeicaoDto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (id != refeicaoDto.Id)
            return BadRequest("O ID da rota não corresponde ao ID da refeição fornecida.");

        var userId = GetUserIdFromClaims();
        var updatedRefeicao = await _refeicaoService.UpdateRefeicaoAsync(id, userId, refeicaoDto);

        if (updatedRefeicao == null)
            return NotFound($"Refeição com ID {id} não encontrada ou não pertence ao usuário.");

        return Ok(updatedRefeicao);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRefeicao(int id)
    {
        var userId = GetUserIdFromClaims();
        var refeicao = await _refeicaoService.GetRefeicaoByIdAsync(id);

        if (refeicao == null || refeicao.UserId != userId)
            return NotFound("Refeição não encontrada ou você não tem permissão para excluí-la.");

        await _refeicaoService.DeleteRefeicaoAsync(id);
        return NoContent();
    }
}