using ControleGlicemia.API.Data;
using ControleGlicemia.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleGlicemia.API.Repositories;

public class RefeicaoRepository : GenericRepository<Refeicao>, IRefeicaoRepository
{
    public RefeicaoRepository(AppDbContext context) : base(context) { }
}
