using AutoMapper;
using ControleGlicemia.API.DTOs.Refeicao;
using ControleGlicemia.API.Mappers;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Repositories;
using ControleGlicemia.API.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ControleGlicemia.API.Tests.Services;

public class RefeicaoServiceTests
{
    private readonly Mock<IRefeicaoRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly RefeicaoService _service;

    public RefeicaoServiceTests()
    {
        _repositoryMock = new Mock<IRefeicaoRepository>();
        var loggerMock = new Mock<ILogger<RefeicaoService>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = config.CreateMapper();

        _service = new RefeicaoService(_repositoryMock.Object, _mapper, loggerMock.Object);
    }

    [Fact]
    public async Task AddRefeicaoAsync_DeveAdicionarRefeicao_QuandoDadosValidos()
    {
        var userId = 1;
        var dto = new CreateRefeicaoDto
        {
            Nome = "Café da manhã",
            Descricao = "Pão com manteiga",
            DataHora = DateTime.UtcNow.AddHours(-2),
            Observacoes = "Teste"
        };

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Refeicao>())).Returns(Task.CompletedTask);

        await _service.AddRefeicaoAsync(userId, dto);

        _repositoryMock.Verify(r => r.AddAsync(It.Is<Refeicao>(r =>
            r.UserId == userId &&
            r.Nome == "Café da manhã" &&
            r.Descricao == "Pão com manteiga"
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateRefeicaoAsync_DeveAtualizarRefeicao_QuandoPertenceAoUsuario()
    {
        var userId = 1;
        var existingRefeicao = new Refeicao
        {
            Id = 1,
            UserId = userId,
            Nome = "Antigo",
            DataHora = DateTime.UtcNow.AddHours(-1)
        };

        var dto = new UpdateRefeicaoDto
        {
            Id = 1,
            Nome = "Novo",
            DataHora = DateTime.UtcNow.AddHours(-1)
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingRefeicao);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Refeicao>())).Returns(Task.CompletedTask);

        var result = await _service.UpdateRefeicaoAsync(1, userId, dto);

        Assert.NotNull(result);
        Assert.Equal("Novo", result.Nome);
    }

    [Fact]
    public async Task UpdateRefeicaoAsync_DeveRetornarNull_QuandoNaoPertenceAoUsuario()
    {
        var existingRefeicao = new Refeicao
        {
            Id = 1,
            UserId = 99,
            Nome = "Outro usuario",
            DataHora = DateTime.UtcNow.AddHours(-1)
        };

        var dto = new UpdateRefeicaoDto
        {
            Id = 1,
            Nome = "Novo",
            DataHora = DateTime.UtcNow.AddHours(-1)
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingRefeicao);

        var result = await _service.UpdateRefeicaoAsync(1, 1, dto);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteRefeicaoAsync_DeveDeletarRefeicao_QuandoExiste()
    {
        var refeicao = new Refeicao { Id = 1, UserId = 1, Nome = "Cafe da Manha" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(refeicao);
        _repositoryMock.Setup(r => r.DeleteAsync(refeicao)).Returns(Task.CompletedTask);

        await _service.DeleteRefeicaoAsync(1, 1);

        _repositoryMock.Verify(r => r.DeleteAsync(refeicao), Times.Once);
    }

    [Fact]
    public async Task DeleteRefeicaoAsync_NaoDeveDeletar_QuandoNaoPertenceAoUsuario()
    {
        var refeicao = new Refeicao { Id = 1, UserId = 99, Nome = "Outro" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(refeicao);

        await _service.DeleteRefeicaoAsync(1, 1);

        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Refeicao>()), Times.Never);
    }

    [Fact]
    public async Task GetRefeicaoPagedAsync_DeveRetornarPaginado()
    {
        var userId = 1;
        var refeicoes = new List<Refeicao>
        {
            new() { Id = 1, UserId = userId, Nome = "Café", DataHora = DateTime.UtcNow },
            new() { Id = 2, UserId = userId, Nome = "Almoço", DataHora = DateTime.UtcNow }
        };

        var pagedResult = new PagedResult<Refeicao>
        {
            Items = refeicoes,
            TotalCount = 2,
            Page = 1,
            PageSize = 20
        };

        _repositoryMock.Setup(r => r.GetPagedByUserIdAsync(userId, 1, 20)).ReturnsAsync(pagedResult);

        var result = await _service.GetRefeicoesPagedAsync(userId, 1, 20);

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
    }

    [Fact]
    public async Task GetRefeicaoByIdAsync_DeveRetornarNull_QuandoNaoExiste()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Refeicao?)null);

        var result = await _service.GetRefeicaoByIdAsync(99, 1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetRefeicaoByIdAsync_DeveRetornarNull_QuandoNaoPertenceAoUsuario()
    {
        var refeicao = new Refeicao { Id = 1, UserId = 99, Nome = "Outro" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(refeicao);

        var result = await _service.GetRefeicaoByIdAsync(1, 1);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateRefeicaoAsync_DeveRetornarNull_QuandoNaoEncontrada()
    {
        var dto = new UpdateRefeicaoDto { Id = 99, Nome = "Teste", DataHora = DateTime.UtcNow };
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Refeicao?)null);

        var result = await _service.UpdateRefeicaoAsync(99, 1, dto);

        Assert.Null(result);
    }
}
