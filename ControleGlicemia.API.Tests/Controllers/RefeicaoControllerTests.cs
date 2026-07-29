using System.Security.Claims;
using ControleGlicemia.API.Controllers;
using ControleGlicemia.API.DTOs.Refeicao;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ControleGlicemia.API.Tests.Controllers;

public class RefeicaoControllerTests
{
    private readonly Mock<IRefeicaoService> _serviceMock;
    private readonly RefeicaoController _controller;

    public RefeicaoControllerTests()
    {
        _serviceMock = new Mock<IRefeicaoService>();
        var loggerMock = new Mock<ILogger<RefeicaoController>>();
        _controller = new RefeicaoController(_serviceMock.Object, loggerMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        }));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        _serviceMock.Setup(s => s.DeleteRefeicaoAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
    }

    [Fact]
    public async Task GetAllRefeicoes_DeveRetornar200()
    {
        _serviceMock.Setup(s => s.GetRefeicoesPagedAsync(1, 1, 20))
            .ReturnsAsync(new PagedResult<RefeicaoDto> { Items = new List<RefeicaoDto>(), TotalCount = 0, Page = 1, PageSize = 20 });

        var result = await _controller.GetAllRefeicoes(1, 20);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.True(response!.Success);
    }

    [Fact]
    public async Task GetRefeicaoById_DeveRetornar200_QuandoExiste()
    {
        _serviceMock.Setup(s => s.GetRefeicaoByIdAsync(1, 1)).ReturnsAsync(new RefeicaoDto { Id = 1, Nome = "Café" });

        var result = await _controller.GetRefeicaoById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.True(response!.Success);
    }

    [Fact]
    public async Task GetRefeicaoById_DeveRetornar404_QuandoNaoExiste()
    {
        _serviceMock.Setup(s => s.GetRefeicaoByIdAsync(99, 1)).ReturnsAsync((RefeicaoDto?)null);

        var result = await _controller.GetRefeicaoById(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task AddRefeicao_DeveRetornar201()
    {
        var dto = new CreateRefeicaoDto { Nome = "Café", DataHora = DateTime.UtcNow };

        var result = await _controller.AddRefeicao(dto);

        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task UpdateRefeicao_DeveRetornar200()
    {
        var dto = new UpdateRefeicaoDto { Id = 1, Nome = "Café", DataHora = DateTime.UtcNow };
        _serviceMock.Setup(s => s.UpdateRefeicaoAsync(1, 1, dto)).ReturnsAsync(new RefeicaoDto());

        var result = await _controller.UpdateRefeicao(1, dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.True(response!.Success);
    }

    [Fact]
    public async Task UpdateRefeicao_DeveRetornar400_QuandoIdDaRotaDiferenteDoBody()
    {
        var dto = new UpdateRefeicaoDto { Id = 2, Nome = "Café", DataHora = DateTime.UtcNow };

        var result = await _controller.UpdateRefeicao(1, dto);

        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = badResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.False(response!.Success);
    }

    [Fact]
    public async Task DeleteRefeicao_DeveRetornar200()
    {
        var result = await _controller.DeleteRefeicao(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.True(response!.Success);
    }
}
