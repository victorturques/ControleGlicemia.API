using ControleGlicemia.API.Data;
using ControleGlicemia.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleGlicemia.API.Repositories;

public class RegistroGlicoseRepository : GenericRepository<RegistroGlicose>, IRegistroGlicoseRepository
{
    public RegistroGlicoseRepository(AppDbContext context) : base(context) { }
}
