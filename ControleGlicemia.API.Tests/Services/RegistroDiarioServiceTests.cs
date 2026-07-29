using AutoMapper;
using ControleGlicemia.API.DTOs.RegistroDiario;
using ControleGlicemia.API.Mappers;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Repositories;
using ControleGlicemia.API.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ControleGlicemia.API.Tests.Services;

public class RegistroDiarioServiceTests
{
    private readonly Mock<IRegistroDiarioRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly RegistroDiarioService _service;

    public RegistroDiarioServiceTests()
    {
        _repositoryMock = new Mock<IRegistroDiarioRepository>();
        var loggerMock = new Mock<ILogger<RegistroDiarioService>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = config.CreateMapper();

        _service = new RegistroDiarioService(_repositoryMock.Object, _mapper, loggerMock.Object);
    }

    [Fact]
    public async Task AddRegistroDiarioAsync_DeveAdicionar_QuandoDadosValidos()
    {
        var userId = 1;
        var dto = new CreateRegistroDiarioDto
        {
            Data = DateTime.UtcNow.Date,
            Observacoes = "Sentindo-me bem"
        };

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<RegistroDiario>())).Returns(Task.CompletedTask);

        await _service.AddRegistroDiarioAsync(userId, dto);

        _repositoryMock.Verify(r => r.AddAsync(It.Is<RegistroDiario>(rd =>
            rd.UserId == userId &&
            rd.Observacoes == "Sentindo-me bem"
        )), Times.Once);
    }

    [Fact]
    public async Task GetRegistrosDiariosPagedAsync_DeveRetornarPaginado()
    {
        var userId = 1;
        var registros = new List<RegistroDiario>
        {
            new() { Id = 1, UserId = userId, Data = DateTime.UtcNow.Date, Observacoes = "Bem" },
            new() { Id = 2, UserId = userId, Data = DateTime.UtcNow.Date, Observacoes = "Normal" }
        };

        var pagedResult = new PagedResult<RegistroDiario>
        {
            Items = registros,
            TotalCount = 2,
            Page = 1,
            PageSize = 20
        };

        _repositoryMock.Setup(r => r.GetPagedByUserIdAsync(userId, 1, 20)).ReturnsAsync(pagedResult);

        var result = await _service.GetRegistrosDiariosPagedAsync(userId, 1, 20);

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
    }

    [Fact]
    public async Task GetRegistroDiarioByIdAsync_DeveRetornar_QuandoExiste()
    {
        var registro = new RegistroDiario
        {
            Id = 1,
            UserId = 1,
            Data = DateTime.UtcNow.Date,
            Observacoes = "Bem"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(registro);

        var result = await _service.GetRegistroDiarioByIdAsync(1, 1);

        Assert.NotNull(result);
        Assert.Equal("Bem", result.Observacoes);
    }

    [Fact]
    public async Task GetRegistroDiarioByIdAsync_DeveRetornarNull_QuandoNaoEncontrado()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((RegistroDiario?)null);

        var result = await _service.GetRegistroDiarioByIdAsync(99, 1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetRegistroDiarioByIdAsync_DeveRetornarNull_QuandoNaoPertenceAoUsuario()
    {
        var registro = new RegistroDiario { Id = 1, UserId = 99, Data = DateTime.UtcNow.Date };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(registro);

        var result = await _service.GetRegistroDiarioByIdAsync(1, 1);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateRegistroDiarioAsync_DeveAtualizar_QuandoValido()
    {
        var existing = new RegistroDiario
        {
            Id = 1,
            UserId = 1,
            Data = DateTime.UtcNow.Date,
            Observacoes = "Antigo"
        };

        var dto = new UpdateRegistroDiarioDto
        {
            Id = 1,
            Data = DateTime.UtcNow.Date,
            Observacoes = "Novo"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<RegistroDiario>())).Returns(Task.CompletedTask);

        var result = await _service.UpdateRegistroDiarioAsync(1, 1, dto);

        Assert.NotNull(result);
        Assert.Equal("Novo", result.Observacoes);
    }

    [Fact]
    public async Task UpdateRegistroDiarioAsync_DeveRetornarNull_QuandoNaoPertenceAoUsuario()
    {
        var existing = new RegistroDiario { Id = 1, UserId = 99, Data = DateTime.UtcNow.Date };
        var dto = new UpdateRegistroDiarioDto { Id = 1, Data = DateTime.UtcNow.Date, Observacoes = "Teste" };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        var result = await _service.UpdateRegistroDiarioAsync(1, 1, dto);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteRegistroDiarioAsync_DeveDeletar_QuandoExiste()
    {
        var registro = new RegistroDiario { Id = 1, UserId = 1, Data = DateTime.UtcNow.Date };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(registro);
        _repositoryMock.Setup(r => r.DeleteAsync(registro)).Returns(Task.CompletedTask);

        await _service.DeleteRegistroDiarioAsync(1, 1);

        _repositoryMock.Verify(r => r.DeleteAsync(registro), Times.Once);
    }

    [Fact]
    public async Task DeleteRegistroDiarioAsync_NaoDeveDeletar_QuandoNaoPertenceAoUsuario()
    {
        var registro = new RegistroDiario { Id = 1, UserId = 99, Data = DateTime.UtcNow.Date };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(registro);

        await _service.DeleteRegistroDiarioAsync(1, 1);

        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<RegistroDiario>()), Times.Never);
    }
}
