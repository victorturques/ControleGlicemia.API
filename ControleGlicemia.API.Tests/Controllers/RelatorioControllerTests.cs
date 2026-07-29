using System.Security.Claims;
using ControleGlicemia.API.Controllers;
using ControleGlicemia.API.DTOs.Relatorio;
using ControleGlicemia.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ControleGlicemia.API.Tests.Controllers;

public class RelatorioControllerTests
{
    private readonly Mock<IRelatorioService> _serviceMock;
    private readonly RelatorioController _controller;

    public RelatorioControllerTests()
    {
        _serviceMock = new Mock<IRelatorioService>();
        var loggerMock = new Mock<ILogger<RelatorioController>>();
        _controller = new RelatorioController(_serviceMock.Object, loggerMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        }));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task GerarRelatorio_DeveRetornar200ComPdf()
    {
        var request = new RelatorioRequestDto
        {
            DataInicio = new DateTime(2026, 7, 1),
            DataFim = new DateTime(2026, 7, 26)
        };
        var pdfBytes = new byte[] { 0x25, 0x50, 0x44, 0x46 };

        _serviceMock.Setup(s => s.GerarRelatorioPdfAsync(1, request)).ReturnsAsync(pdfBytes);

        var result = await _controller.GerarRelatorio(request);

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/pdf", fileResult.ContentType);
        Assert.Equal(pdfBytes, fileResult.FileContents);
    }

    [Fact]
    public async Task GerarRelatorio_DeveRetornar400_QuandoModelStateInvalido()
    {
        _controller.ModelState.AddModelError("DataInicio", "Obrigatório");
        var request = new RelatorioRequestDto();

        var result = await _controller.GerarRelatorio(request);

        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = badResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.False(response!.Success);
    }
}
