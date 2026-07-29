using System.Security.Claims;
using ControleGlicemia.API.Controllers;
using ControleGlicemia.API.DTOs.RegistroGlicose;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ControleGlicemia.API.Tests.Controllers;

public class RegistroGlicoseControllerTests
{
    private readonly Mock<IRegistroGlicoseService> _serviceMock;
    private readonly RegistroGlicoseController _controller;

    public RegistroGlicoseControllerTests()
    {
        _serviceMock = new Mock<IRegistroGlicoseService>();
        var loggerMock = new Mock<ILogger<RegistroGlicoseController>>();
        _controller = new RegistroGlicoseController(_serviceMock.Object, loggerMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        }));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        _serviceMock.Setup(s => s.DeleteRegistroGlicoseAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
    }

    [Fact]
    public async Task GetAllRegistrosGlicose_DeveRetornar200()
    {
        var pagedResult = new PagedResult<RegistroGlicoseDto>
        {
            Items = new List<RegistroGlicoseDto>(),
            TotalCount = 0,
            Page = 1,
            PageSize = 20
        };
        _serviceMock.Setup(s => s.GetRegistrosGlicosePagedAsync(1, 1, 20)).ReturnsAsync(pagedResult);

        var result = await _controller.GetAllRegistrosGlicose(1, 20);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task GetRegistroGlicoseById_DeveRetornar200_QuandoExiste()
    {
        var dto = new RegistroGlicoseDto { Id = 1, Valor = 100 };
        _serviceMock.Setup(s => s.GetRegistroGlicoseByIdAsync(1, 1)).ReturnsAsync(dto);

        var result = await _controller.GetRegistroGlicoseById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task GetRegistroGlicoseById_DeveRetornar404_QuandoNaoExiste()
    {
        _serviceMock.Setup(s => s.GetRegistroGlicoseByIdAsync(99, 1)).ReturnsAsync((RegistroGlicoseDto?)null);

        var result = await _controller.GetRegistroGlicoseById(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task AddRegistroGlicose_DeveRetornar201()
    {
        var dto = new CreateRegistroGlicoseDto
        {
            Valor = 100,
            MedidoEm = DateTime.UtcNow.AddMinutes(-10),
            MomentoMedicao = MomentoMedicao.PreCafe
        };

        var result = await _controller.AddRegistroGlicose(dto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var response = createdResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task UpdateRegistroGlicose_DeveRetornar200()
    {
        var dto = new UpdateRegistroGlicoseDto
        {
            Id = 1,
            Valor = 120,
            MedidoEm = DateTime.UtcNow.AddMinutes(-10),
            MomentoMedicao = MomentoMedicao.PreCafe
        };
        _serviceMock.Setup(s => s.UpdateRegistroGlicoseAsync(1, 1, dto)).ReturnsAsync(new RegistroGlicoseDto());

        var result = await _controller.UpdateRegistroGlicose(1, dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task UpdateRegistroGlicose_DeveRetornar400_QuandoIdDaRotaDiferenteDoBody()
    {
        var dto = new UpdateRegistroGlicoseDto { Id = 2, Valor = 120, MedidoEm = DateTime.UtcNow, MomentoMedicao = MomentoMedicao.PreCafe };

        var result = await _controller.UpdateRegistroGlicose(1, dto);

        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = badResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.False(response.Success);
    }

    [Fact]
    public async Task UpdateRegistroGlicose_DeveRetornar404_QuandoNaoEncontrado()
    {
        var dto = new UpdateRegistroGlicoseDto { Id = 99, Valor = 120, MedidoEm = DateTime.UtcNow, MomentoMedicao = MomentoMedicao.PreCafe };
        _serviceMock.Setup(s => s.UpdateRegistroGlicoseAsync(99, 1, dto)).ReturnsAsync((RegistroGlicoseDto?)null);

        var result = await _controller.UpdateRegistroGlicose(99, dto);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteRegistroGlicose_DeveRetornar200()
    {
        var result = await _controller.DeleteRegistroGlicose(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.True(response.Success);
    }
}
