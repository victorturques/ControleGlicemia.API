using ControleGlicemia.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleGlicemia.API.Repositories;

public interface IRefeicaoRepository
{
    Task<IEnumerable<Refeicao>> GetAllByUserIdAsync(int userId);
    Task<Refeicao?> GetByIdAsync(int id);
    Task AddAsync(Refeicao refeicao);
    Task DeleteAsync(Refeicao refeicao);
    Task UpdateAsync(Refeicao entity);
}