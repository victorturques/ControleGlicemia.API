using ControleGlicemia.API.Data;
using ControleGlicemia.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleGlicemia.API.Repositories;

public class RegistroDiarioRepository : GenericRepository<RegistroDiario>, IRegistroDiarioRepository
{
    public RegistroDiarioRepository(AppDbContext context) : base(context) { }
}
