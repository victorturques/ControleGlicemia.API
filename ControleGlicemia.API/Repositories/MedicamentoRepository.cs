using ControleGlicemia.API.Data;
using ControleGlicemia.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleGlicemia.API.Repositories;

public class MedicamentoRepository : GenericRepository<Medicamento>, IMedicamentoRepository
{
    public MedicamentoRepository(AppDbContext context) : base(context) { }
}
