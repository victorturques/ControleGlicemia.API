using ControleGlicemia.API.Data;
using ControleGlicemia.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleGlicemia.API.Repositories;

public class RefeicaoRepository : IRefeicaoRepository
{
    private readonly AppDbContext _context;

    public RefeicaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Refeicao>> GetAllByUserIdAsync(int userId)
        => await _context.Refeicoes.Where(r => r.UserId == userId).ToListAsync();

    public async Task<Refeicao?> GetByIdAsync(int id)
        => await _context.Refeicoes.FindAsync(id);

    public async Task AddAsync(Refeicao refeicao)
    {
        await _context.Refeicoes.AddAsync(refeicao);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Refeicao refeicao)
    {
        _context.Refeicoes.Remove(refeicao);
        await _context.SaveChangesAsync();
    }

       public async Task UpdateAsync(Refeicao refeicao)
    {
        _context.Refeicoes.Update(refeicao);
        await _context.SaveChangesAsync();
    }
}