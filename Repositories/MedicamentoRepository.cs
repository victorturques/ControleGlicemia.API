using ControleGlicemia.API.Data;
using ControleGlicemia.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControleGlicemia.API.Repositories;

public class MedicamentoRepository : IMedicamentoRepository
{
    private readonly AppDbContext _context;

    public MedicamentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Medicamento>> GetAllByUserIdAsync(int userId)
        => await _context.Medicamentos.Where(m => m.UserId == userId).ToListAsync();

    public async Task<Medicamento?> GetByIdAsync(int id)
        => await _context.Medicamentos.FindAsync(id);

    public async Task AddAsync(Medicamento medicamento)
    {
        await _context.Medicamentos.AddAsync(medicamento);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Medicamento medicamento)
    {
        _context.Medicamentos.Remove(medicamento);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Medicamento medicamento)
    {
        _context.Medicamentos.Update(medicamento);
        await _context.SaveChangesAsync();
    }
}