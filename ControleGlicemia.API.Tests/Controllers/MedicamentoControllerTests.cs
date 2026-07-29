using System.Security.Claims;
using ControleGlicemia.API.Controllers;
using ControleGlicemia.API.DTOs.Medicamento;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ControleGlicemia.API.Tests.Controllers;

public class MedicamentoControllerTests
{
    private readonly Mock<IMedicamentoService> _serviceMock;
    private readonly MedicamentoController _controller;

    public MedicamentoControllerTests()
    {
        _serviceMock = new Mock<IMedicamentoService>();
        var loggerMock = new Mock<ILogger<MedicamentoController>>();
        _controller = new MedicamentoController(_serviceMock.Object, loggerMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        }));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        _serviceMock.Setup(s => s.DeleteMedicamentoAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
    }

    [Fact]
    public async Task GetAllByUserId_DeveRetornar200()
    {
        _serviceMock.Setup(s => s.GetMedicamentosPagedAsync(1, 1, 20))
            .ReturnsAsync(new PagedResult<MedicamentoDto> { Items = new List<MedicamentoDto>(), TotalCount = 0, Page = 1, PageSize = 20 });

        var result = await _controller.GetAllByUserId(1, 20);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task GetById_DeveRetornar200_QuandoExiste()
    {
        _serviceMock.Setup(s => s.GetMedicamentoByIdAsync(1, 1)).ReturnsAsync(new MedicamentoDto { Id = 1, Nome = "Insulina" });

        var result = await _controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.True(response!.Success);
    }

    [Fact]
    public async Task GetById_DeveRetornar404_QuandoNaoExiste()
    {
        _serviceMock.Setup(s => s.GetMedicamentoByIdAsync(99, 1)).ReturnsAsync((MedicamentoDto?)null);

        var result = await _controller.GetById(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Add_DeveRetornar201()
    {
        var dto = new CreateMedicamentoDto { Nome = "Insulina", Dose = 10, TomadoEm = DateTime.UtcNow };

        var result = await _controller.Add(dto);

        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task Update_DeveRetornar200()
    {
        var dto = new UpdateMedicamentoDto { Id = 1, Nome = "Insulina", Dose = 10, TomadoEm = DateTime.UtcNow };
        _serviceMock.Setup(s => s.UpdateMedicamentoAsync(1, 1, dto)).ReturnsAsync(new MedicamentoDto());

        var result = await _controller.Update(1, dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response2 = okResult.Value as ApiResponse<object>;
        Assert.NotNull(response2);
        Assert.True(response2!.Success);
    }

    [Fact]
    public async Task Update_DeveRetornar400_QuandoIdDaRotaDiferenteDoBody()
    {
        var dto = new UpdateMedicamentoDto { Id = 2, Nome = "Insulina", Dose = 10, TomadoEm = DateTime.UtcNow };

        var result = await _controller.Update(1, dto);

        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = badResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.False(response.Success);
    }

    [Fact]
    public async Task Delete_DeveRetornar200()
    {
        var result = await _controller.Delete(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response3 = okResult.Value as ApiResponse<object>;
        Assert.NotNull(response3);
        Assert.True(response3!.Success);
    }
}
