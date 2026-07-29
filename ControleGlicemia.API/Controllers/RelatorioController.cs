using ControleGlicemia.API.Extensions;
using ControleGlicemia.API.DTOs.Relatorio;
using ControleGlicemia.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleGlicemia.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RelatorioController : ControllerBase
{
    private readonly IRelatorioService _relatorioService;
    private readonly ILogger<RelatorioController> _logger;

    public RelatorioController(IRelatorioService relatorioService, ILogger<RelatorioController> logger)
    {
        _relatorioService = relatorioService;
        _logger = logger;
    }

    [HttpPost("gerar")]
    public async Task<IActionResult> GerarRelatorio([FromBody] RelatorioRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Dados inválidos.", ModelState.Values
                .SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));

        var userId = User.GetUserId();
        var pdfBytes = await _relatorioService.GerarRelatorioPdfAsync(userId, request);

        return File(pdfBytes, "application/pdf", $"relatorio_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf");
    }
}
