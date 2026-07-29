using ControleGlicemia.API.Data;
using ControleGlicemia.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleGlicemia.API.Repositories;

public class RelatorioRepository : IRelatorioRepository
{
    private readonly AppDbContext _context;

    public RelatorioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<List<RegistroGlicose>> GetRegistrosGlicoseByPeriodAsync(int userId, DateTime dataInicio, DateTime dataFim)
    {
        return await _context.RegistrosGlicose
            .AsNoTracking()
            .Where(r => r.UserId == userId && r.MedidoEm.Date >= dataInicio && r.MedidoEm.Date <= dataFim)
            .OrderBy(r => r.MedidoEm)
            .ToListAsync();
    }

    public async Task<List<Medicamento>> GetMedicamentosByPeriodAsync(int userId, DateTime dataInicio, DateTime dataFim)
    {
        return await _context.Medicamentos
            .AsNoTracking()
            .Where(m => m.UserId == userId && m.TomadoEm.Date >= dataInicio && m.TomadoEm.Date <= dataFim)
            .OrderBy(m => m.TomadoEm)
            .ToListAsync();
    }

    public async Task<List<RegistroDiario>> GetRegistrosDiariosByPeriodAsync(int userId, DateTime dataInicio, DateTime dataFim)
    {
        return await _context.RegistrosDiarios
            .AsNoTracking()
            .Where(r => r.UserId == userId && r.Data.Date >= dataInicio && r.Data.Date <= dataFim)
            .OrderBy(r => r.Data)
            .ToListAsync();
    }

    public async Task<List<Refeicao>> GetRefeicoesByPeriodAsync(int userId, DateTime dataInicio, DateTime dataFim)
    {
        return await _context.Refeicoes
            .AsNoTracking()
            .Where(r => r.UserId == userId && r.DataHora.Date >= dataInicio && r.DataHora.Date <= dataFim)
            .OrderBy(r => r.DataHora)
            .ToListAsync();
    }
}
