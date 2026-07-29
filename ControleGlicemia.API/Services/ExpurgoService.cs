using ControleGlicemia.API.Data;
using ControleGlicemia.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleGlicemia.API.Services;

public class ExpurgoService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ExpurgoService> _logger;
    private static readonly TimeSpan PeriodoRetencao = TimeSpan.FromDays(90);
    private static readonly TimeSpan IntervaloExecucao = TimeSpan.FromHours(24);

    public ExpurgoService(IServiceScopeFactory scopeFactory, ILogger<ExpurgoService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ExpurgoService iniciado. Período de retenção: {Dias} dias", PeriodoRetencao.Days);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExecutarExpurgoAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar expurgo automático.");
            }

            await Task.Delay(IntervaloExecucao, stoppingToken);
        }
    }

    private async Task ExecutarExpurgoAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var corte = DateTime.UtcNow.AddDays(-PeriodoRetencao.Days);
        var total = 0;

        total += await context.Set<RegistroGlicose>()
            .IgnoreQueryFilters()
            .Where(e => e.DeletedAt != null && e.DeletedAt < corte)
            .ExecuteDeleteAsync(stoppingToken);

        total += await context.Set<Medicamento>()
            .IgnoreQueryFilters()
            .Where(e => e.DeletedAt != null && e.DeletedAt < corte)
            .ExecuteDeleteAsync(stoppingToken);

        total += await context.Set<Refeicao>()
            .IgnoreQueryFilters()
            .Where(e => e.DeletedAt != null && e.DeletedAt < corte)
            .ExecuteDeleteAsync(stoppingToken);

        total += await context.Set<RegistroDiario>()
            .IgnoreQueryFilters()
            .Where(e => e.DeletedAt != null && e.DeletedAt < corte)
            .ExecuteDeleteAsync(stoppingToken);

        total += await context.Set<User>()
            .IgnoreQueryFilters()
            .Where(e => e.DeletedAt != null && e.DeletedAt < corte)
            .ExecuteDeleteAsync(stoppingToken);

        if (total > 0)
            _logger.LogInformation("Expurgo concluído: {Total} registro(s) removido(s) (retenção de {Dias} dias).", total, PeriodoRetencao.Days);
    }
}
