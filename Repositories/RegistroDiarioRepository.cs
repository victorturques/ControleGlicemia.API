using ControleGlicemia.API.Data;
using ControleGlicemia.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControleGlicemia.API.Repositories;

public class RegistroDiarioRepository : IRegistroDiarioRepository
{
    private readonly AppDbContext _context;

    public RegistroDiarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RegistroDiario>> GetAllByUserIdAsync(int userId)
        => await _context.RegistrosDiarios.Where(r => r.UserId == userId).ToListAsync();

    public async Task<RegistroDiario?> GetByIdAsync(int id)
        => await _context.RegistrosDiarios.FindAsync(id);

    public async Task AddAsync(RegistroDiario registro)
    {
        await _context.RegistrosDiarios.AddAsync(registro);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(RegistroDiario registro)
    {
        _context.RegistrosDiarios.Remove(registro);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RegistroDiario registro)
    {
        _context.RegistrosDiarios.Update(registro);
        await _context.SaveChangesAsync();
    }
}