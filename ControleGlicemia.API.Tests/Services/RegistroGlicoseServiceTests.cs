using AutoMapper;
using ControleGlicemia.API.DTOs.RegistroGlicose;
using ControleGlicemia.API.Mappers;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Repositories;
using ControleGlicemia.API.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ControleGlicemia.API.Tests.Services;

public class RegistroGlicoseServiceTests
{
    private readonly Mock<IRegistroGlicoseRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly RegistroGlicoseService _service;

    public RegistroGlicoseServiceTests()
    {
        _repositoryMock = new Mock<IRegistroGlicoseRepository>();
        var loggerMock = new Mock<ILogger<RegistroGlicoseService>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = config.CreateMapper();

        _service = new RegistroGlicoseService(_repositoryMock.Object, _mapper, loggerMock.Object);
    }

    [Fact]
    public async Task AddRegistroGlicoseAsync_DeveAdicionarRegistro_QuandoDadosValidos()
    {
        var userId = 1;
        var dto = new CreateRegistroGlicoseDto
        {
            Valor = 120,
            MedidoEm = DateTime.UtcNow.AddMinutes(-30),
            MomentoMedicao = MomentoMedicao.PreCafe,
            Observacoes = "Teste"
        };

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<RegistroGlicose>())).Returns(Task.CompletedTask);

        await _service.AddRegistroGlicoseAsync(userId, dto);

        _repositoryMock.Verify(r => r.AddAsync(It.Is<RegistroGlicose>(rg =>
            rg.UserId == userId &&
            rg.Valor == 120 &&
            rg.MomentoMedicao == MomentoMedicao.PreCafe
        )), Times.Once);
    }

    [Fact]
    public async Task GetRegistroGlicoseByIdAsync_DeveRetornarRegistro_QuandoExiste()
    {
        var registro = new RegistroGlicose
        {
            Id = 1,
            UserId = 1,
            Valor = 120,
            MedidoEm = DateTime.UtcNow,
            MomentoMedicao = MomentoMedicao.PreCafe
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(registro);

        var result = await _service.GetRegistroGlicoseByIdAsync(1, 1);

        Assert.NotNull(result);
        Assert.Equal(120, result.Valor);
    }

    [Fact]
    public async Task DeleteRegistroGlicoseAsync_DeveDeletarRegistro_QuandoExiste()
    {
        var registro = new RegistroGlicose { Id = 1, UserId = 1 };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(registro);
        _repositoryMock.Setup(r => r.DeleteAsync(registro)).Returns(Task.CompletedTask);

        await _service.DeleteRegistroGlicoseAsync(1, 1);

        _repositoryMock.Verify(r => r.DeleteAsync(registro), Times.Once);
    }

    [Fact]
    public async Task DeleteRegistroGlicoseAsync_NaoDeveLancarExcecao_QuandoRegistroNaoExiste()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((RegistroGlicose?)null);

        await _service.DeleteRegistroGlicoseAsync(99, 1);

        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<RegistroGlicose>()), Times.Never);
    }

    [Fact]
    public async Task DeleteRegistroGlicoseAsync_NaoDeveDeletar_QuandoNaoPertenceAoUsuario()
    {
        var registro = new RegistroGlicose { Id = 1, UserId = 99 };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(registro);

        await _service.DeleteRegistroGlicoseAsync(1, 1);

        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<RegistroGlicose>()), Times.Never);
    }

    [Fact]
    public async Task GetRegistroGlicosePagedAsync_DeveRetornarPaginado()
    {
        var userId = 1;
        var registros = new List<RegistroGlicose>
        {
            new() { Id = 1, UserId = userId, Valor = 100, MedidoEm = DateTime.UtcNow, MomentoMedicao = MomentoMedicao.PreCafe },
            new() { Id = 2, UserId = userId, Valor = 120, MedidoEm = DateTime.UtcNow, MomentoMedicao = MomentoMedicao.PosCafe }
        };

        var pagedResult = new PagedResult<RegistroGlicose>
        {
            Items = registros,
            TotalCount = 2,
            Page = 1,
            PageSize = 20
        };

        _repositoryMock.Setup(r => r.GetPagedByUserIdAsync(userId, 1, 20)).ReturnsAsync(pagedResult);

        var result = await _service.GetRegistrosGlicosePagedAsync(userId, 1, 20);

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(1, result.Page);
    }

    [Fact]
    public async Task GetRegistroGlicoseByIdAsync_DeveRetornarNull_QuandoRegistroNaoExiste()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((RegistroGlicose?)null);

        var result = await _service.GetRegistroGlicoseByIdAsync(99, 1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetRegistroGlicoseByIdAsync_DeveRetornarNull_QuandoNaoPertenceAoUsuario()
    {
        var registro = new RegistroGlicose { Id = 1, UserId = 99, Valor = 100 };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(registro);

        var result = await _service.GetRegistroGlicoseByIdAsync(1, 1);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateRegistroGlicoseAsync_DeveRetornarNull_QuandoNaoPertenceAoUsuario()
    {
        var registro = new RegistroGlicose { Id = 1, UserId = 99, Valor = 100 };
        var dto = new UpdateRegistroGlicoseDto { Id = 1, Valor = 120, MedidoEm = DateTime.UtcNow, MomentoMedicao = MomentoMedicao.PreCafe };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(registro);

        var result = await _service.UpdateRegistroGlicoseAsync(1, 1, dto);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateRegistroGlicoseAsync_DeveRetornarNull_QuandoRegistroNaoExiste()
    {
        var dto = new UpdateRegistroGlicoseDto { Id = 99, Valor = 120, MedidoEm = DateTime.UtcNow, MomentoMedicao = MomentoMedicao.PreCafe };

        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((RegistroGlicose?)null);

        var result = await _service.UpdateRegistroGlicoseAsync(99, 1, dto);

        Assert.Null(result);
    }
}
