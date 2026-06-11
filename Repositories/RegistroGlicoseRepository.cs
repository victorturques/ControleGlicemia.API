using ControleGlicemia.API.Data;
using ControleGlicemia.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControleGlicemia.API.Repositories;

public class RegistroGlicoseRepository : IRegistroGlicoseRepository
{
    private readonly AppDbContext _context;

    public RegistroGlicoseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RegistroGlicose>> GetAllByUserIdAsync(int userId)
        => await _context.RegistrosGlicose.Where(r => r.UserId == userId).ToListAsync();

    public async Task<RegistroGlicose?> GetByIdAsync(int id)
        => await _context.RegistrosGlicose.FindAsync(id);

    public async Task AddAsync(RegistroGlicose registro)
    {
        await _context.RegistrosGlicose.AddAsync(registro);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(RegistroGlicose registro)
    {
        _context.RegistrosGlicose.Remove(registro);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RegistroGlicose registro)
    {
        _context.RegistrosGlicose.Update(registro);
        await _context.SaveChangesAsync();
    }
}