using System.ComponentModel.DataAnnotations;
using ControleGlicemia.API.DTOs.Relatorio;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Repositories;
using ControleGlicemia.API.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ControleGlicemia.API.Tests.Services;

public class RelatorioServiceTests
{
    private readonly Mock<IRelatorioRepository> _repositoryMock;
    private readonly RelatorioService _service;

    public RelatorioServiceTests()
    {
        _repositoryMock = new Mock<IRelatorioRepository>();
        var loggerMock = new Mock<ILogger<RelatorioService>>();

        _service = new RelatorioService(_repositoryMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task GerarRelatorioPdfAsync_DeveGerarPdf_QuandoDadosValidos()
    {
        var userId = 1;
        var request = new RelatorioRequestDto
        {
            DataInicio = new DateTime(2026, 7, 1),
            DataFim = new DateTime(2026, 7, 26),
            NomeMedico = "Dr. João"
        };

        var user = new User
        {
            Id = userId,
            Nome = "Paciente Teste",
            Email = "teste@email.com",
            SenhaHash = "hash",
            GlicemiaMinima = 70,
            GlicemiaMaxima = 140
        };

        var registrosGlicose = new List<RegistroGlicose>
        {
            new() { Id = 1, UserId = userId, Valor = 100, MedidoEm = new DateTime(2026, 7, 15, 8, 0, 0), MomentoMedicao = MomentoMedicao.PreCafe },
            new() { Id = 2, UserId = userId, Valor = 150, MedidoEm = new DateTime(2026, 7, 15, 12, 0, 0), MomentoMedicao = MomentoMedicao.PosAlmoco }
        };

        var medicamentos = new List<Medicamento>
        {
            new() { Id = 1, UserId = userId, Nome = "Insulina", Dose = 10, TomadoEm = new DateTime(2026, 7, 15, 8, 0, 0) }
        };

        var registrosDiarios = new List<RegistroDiario>
        {
            new() { Id = 1, UserId = userId, Data = new DateTime(2026, 7, 15), Observacoes = "Bem" }
        };

        _repositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);
        _repositoryMock.Setup(r => r.GetRegistrosGlicoseByPeriodAsync(userId, It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(registrosGlicose);
        _repositoryMock.Setup(r => r.GetMedicamentosByPeriodAsync(userId, It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(medicamentos);
        _repositoryMock.Setup(r => r.GetRegistrosDiariosByPeriodAsync(userId, It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(registrosDiarios);

        var result = await _service.GerarRelatorioPdfAsync(userId, request);

        Assert.NotNull(result);
        Assert.True(result.Length > 0, "PDF gerado deve ter conteúdo");
    }

    [Fact]
    public async Task GerarRelatorioPdfAsync_DeveGerarPdf_QuandoSemRegistros()
    {
        var userId = 1;
        var request = new RelatorioRequestDto
        {
            DataInicio = new DateTime(2026, 7, 1),
            DataFim = new DateTime(2026, 7, 26)
        };

        var user = new User
        {
            Id = userId,
            Nome = "Paciente Teste",
            Email = "teste@email.com",
            SenhaHash = "hash",
            GlicemiaMinima = 70,
            GlicemiaMaxima = 140
        };

        _repositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);
        _repositoryMock.Setup(r => r.GetRegistrosGlicoseByPeriodAsync(userId, It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(new List<RegistroGlicose>());
        _repositoryMock.Setup(r => r.GetMedicamentosByPeriodAsync(userId, It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(new List<Medicamento>());
        _repositoryMock.Setup(r => r.GetRegistrosDiariosByPeriodAsync(userId, It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(new List<RegistroDiario>());

        var result = await _service.GerarRelatorioPdfAsync(userId, request);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public async Task GerarRelatorioPdfAsync_DeveLancarExcecao_QuandoDataInicioDefault()
    {
        var request = new RelatorioRequestDto
        {
            DataInicio = default,
            DataFim = new DateTime(2026, 7, 26)
        };

        await Assert.ThrowsAsync<ValidationException>(
            () => _service.GerarRelatorioPdfAsync(1, request));
    }

    [Fact]
    public async Task GerarRelatorioPdfAsync_DeveLancarExcecao_QuandoDataFimMenorQueInicio()
    {
        var request = new RelatorioRequestDto
        {
            DataInicio = new DateTime(2026, 7, 26),
            DataFim = new DateTime(2026, 7, 1)
        };

        await Assert.ThrowsAsync<ValidationException>(
            () => _service.GerarRelatorioPdfAsync(1, request));
    }

    [Fact]
    public async Task GerarRelatorioPdfAsync_DeveLancarExcecao_QuandoUsuarioNaoEncontrado()
    {
        var request = new RelatorioRequestDto
        {
            DataInicio = new DateTime(2026, 7, 1),
            DataFim = new DateTime(2026, 7, 26)
        };

        _repositoryMock.Setup(r => r.GetUserByIdAsync(999)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.GerarRelatorioPdfAsync(999, request));
    }
}
