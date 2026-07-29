using System.Security.Claims;
using ControleGlicemia.API.Controllers;
using ControleGlicemia.API.DTOs.RegistroDiario;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ControleGlicemia.API.Tests.Controllers;

public class RegistroDiarioControllerTests
{
    private readonly Mock<IRegistroDiarioService> _serviceMock;
    private readonly RegistroDiarioController _controller;

    public RegistroDiarioControllerTests()
    {
        _serviceMock = new Mock<IRegistroDiarioService>();
        var loggerMock = new Mock<ILogger<RegistroDiarioController>>();
        _controller = new RegistroDiarioController(_serviceMock.Object, loggerMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        }));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        _serviceMock.Setup(s => s.DeleteRegistroDiarioAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
    }

    [Fact]
    public async Task GetAllByUserId_DeveRetornar200()
    {
        _serviceMock.Setup(s => s.GetRegistrosDiariosPagedAsync(1, 1, 20))
            .ReturnsAsync(new PagedResult<RegistroDiarioDto> { Items = new List<RegistroDiarioDto>(), TotalCount = 0, Page = 1, PageSize = 20 });

        var result = await _controller.GetAllByUserId(1, 20);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.True(response!.Success);
    }

    [Fact]
    public async Task GetById_DeveRetornar200_QuandoExiste()
    {
        _serviceMock.Setup(s => s.GetRegistroDiarioByIdAsync(1, 1)).ReturnsAsync(new RegistroDiarioDto { Id = 1 });

        var result = await _controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.True(response!.Success);
    }

    [Fact]
    public async Task GetById_DeveRetornar404_QuandoNaoExiste()
    {
        _serviceMock.Setup(s => s.GetRegistroDiarioByIdAsync(99, 1)).ReturnsAsync((RegistroDiarioDto?)null);

        var result = await _controller.GetById(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Add_DeveRetornar201()
    {
        var dto = new CreateRegistroDiarioDto { Data = DateTime.UtcNow.Date, Observacoes = "Bem" };

        var result = await _controller.Add(dto);

        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task Update_DeveRetornar200()
    {
        var dto = new UpdateRegistroDiarioDto { Id = 1, Data = DateTime.UtcNow.Date };
        _serviceMock.Setup(s => s.UpdateRegistroDiarioAsync(1, 1, dto)).ReturnsAsync(new RegistroDiarioDto());

        var result = await _controller.Update(1, dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.True(response!.Success);
    }

    [Fact]
    public async Task Update_DeveRetornar400_QuandoIdDaRotaDiferenteDoBody()
    {
        var dto = new UpdateRegistroDiarioDto { Id = 2, Data = DateTime.UtcNow.Date };

        var result = await _controller.Update(1, dto);

        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = badResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.False(response!.Success);
    }

    [Fact]
    public async Task Delete_DeveRetornar200()
    {
        var result = await _controller.Delete(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.True(response!.Success);
    }
}
