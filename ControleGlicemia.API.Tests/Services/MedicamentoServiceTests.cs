using AutoMapper;
using ControleGlicemia.API.DTOs.Medicamento;
using ControleGlicemia.API.Mappers;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Repositories;
using ControleGlicemia.API.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ControleGlicemia.API.Tests.Services;

public class MedicamentoServiceTests
{
    private readonly Mock<IMedicamentoRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly MedicamentoService _service;

    public MedicamentoServiceTests()
    {
        _repositoryMock = new Mock<IMedicamentoRepository>();
        var loggerMock = new Mock<ILogger<MedicamentoService>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = config.CreateMapper();

        _service = new MedicamentoService(_repositoryMock.Object, _mapper, loggerMock.Object);
    }

    [Fact]
    public async Task AddMedicamentoAsync_DeveAdicionar_QuandoDadosValidos()
    {
        var userId = 1;
        var dto = new CreateMedicamentoDto
        {
            Nome = "Insulina",
            Dose = 10.0,
            TomadoEm = DateTime.UtcNow.AddHours(-1)
        };

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Medicamento>())).Returns(Task.CompletedTask);

        await _service.AddMedicamentoAsync(userId, dto);

        _repositoryMock.Verify(r => r.AddAsync(It.Is<Medicamento>(m =>
            m.UserId == userId &&
            m.Nome == "Insulina" &&
            m.Dose == 10.0
        )), Times.Once);
    }

    [Fact]
    public async Task GetMedicamentosPagedAsync_DeveRetornarPaginado()
    {
        var userId = 1;
        var medicamentos = new List<Medicamento>
        {
            new() { Id = 1, UserId = userId, Nome = "Insulina", Dose = 10, TomadoEm = DateTime.UtcNow },
            new() { Id = 2, UserId = userId, Nome = "Metformina", Dose = 500, TomadoEm = DateTime.UtcNow }
        };

        var pagedResult = new PagedResult<Medicamento>
        {
            Items = medicamentos,
            TotalCount = 2,
            Page = 1,
            PageSize = 20
        };

        _repositoryMock.Setup(r => r.GetPagedByUserIdAsync(userId, 1, 20)).ReturnsAsync(pagedResult);

        var result = await _service.GetMedicamentosPagedAsync(userId, 1, 20);

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
    }

    [Fact]
    public async Task GetMedicamentoByIdAsync_DeveRetornar_QuandoExiste()
    {
        var medicamento = new Medicamento
        {
            Id = 1,
            UserId = 1,
            Nome = "Insulina",
            Dose = 10,
            TomadoEm = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(medicamento);

        var result = await _service.GetMedicamentoByIdAsync(1, 1);

        Assert.NotNull(result);
        Assert.Equal("Insulina", result.Nome);
    }

    [Fact]
    public async Task GetMedicamentoByIdAsync_DeveRetornarNull_QuandoNaoEncontrado()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Medicamento?)null);

        var result = await _service.GetMedicamentoByIdAsync(99, 1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetMedicamentoByIdAsync_DeveRetornarNull_QuandoNaoPertenceAoUsuario()
    {
        var medicamento = new Medicamento { Id = 1, UserId = 99, Nome = "Insulina", Dose = 10 };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(medicamento);

        var result = await _service.GetMedicamentoByIdAsync(1, 1);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateMedicamentoAsync_DeveAtualizar_QuandoValido()
    {
        var existing = new Medicamento
        {
            Id = 1,
            UserId = 1,
            Nome = "Insulina",
            Dose = 10,
            TomadoEm = DateTime.UtcNow.AddHours(-2)
        };

        var dto = new UpdateMedicamentoDto
        {
            Id = 1,
            Nome = "Insulina Nova",
            Dose = 20,
            TomadoEm = DateTime.UtcNow.AddHours(-1)
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Medicamento>())).Returns(Task.CompletedTask);

        var result = await _service.UpdateMedicamentoAsync(1, 1, dto);

        Assert.NotNull(result);
        Assert.Equal("Insulina Nova", result.Nome);
    }

    [Fact]
    public async Task UpdateMedicamentoAsync_DeveRetornarNull_QuandoNaoPertenceAoUsuario()
    {
        var existing = new Medicamento { Id = 1, UserId = 99, Nome = "Insulina", Dose = 10 };
        var dto = new UpdateMedicamentoDto { Id = 1, Nome = "Nova", Dose = 20, TomadoEm = DateTime.UtcNow };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        var result = await _service.UpdateMedicamentoAsync(1, 1, dto);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteMedicamentoAsync_DeveDeletar_QuandoExiste()
    {
        var medicamento = new Medicamento { Id = 1, UserId = 1, Nome = "Insulina", Dose = 10 };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(medicamento);
        _repositoryMock.Setup(r => r.DeleteAsync(medicamento)).Returns(Task.CompletedTask);

        await _service.DeleteMedicamentoAsync(1, 1);

        _repositoryMock.Verify(r => r.DeleteAsync(medicamento), Times.Once);
    }

    [Fact]
    public async Task DeleteMedicamentoAsync_NaoDeveDeletar_QuandoNaoPertenceAoUsuario()
    {
        var medicamento = new Medicamento { Id = 1, UserId = 99, Nome = "Insulina", Dose = 10 };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(medicamento);

        await _service.DeleteMedicamentoAsync(1, 1);

        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Medicamento>()), Times.Never);
    }
}
