using ControleGlicemia.API.DTOs.Relatorio;
using ControleGlicemia.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ControleGlicemia.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RelatorioController : ControllerBase
{
    private readonly IRelatorioService _relatorioService;

    public RelatorioController(IRelatorioService relatorioService)
    {
        _relatorioService = relatorioService;
    }

    private int GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("User ID not found or invalid in token.");

        return userId;
    }

    [HttpPost("gerar")]
    public async Task<IActionResult> GerarRelatorio([FromBody] RelatorioRequestDto request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var userId = GetUserIdFromClaims();
        var pdfBytes = await _relatorioService.GerarRelatorioPdfAsync(userId, request);

        return File(pdfBytes, "application/pdf", $"relatorio_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf");
    }
}