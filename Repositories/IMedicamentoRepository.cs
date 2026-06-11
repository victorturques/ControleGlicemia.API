using ControleGlicemia.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleGlicemia.API.Repositories;

public interface IMedicamentoRepository
{
    // Task<IEnumerable<Medicamento>> GetAllMedicamentosByUserIdAsync(int userId); // Remover esta linha
    Task<Medicamento?> GetByIdAsync(int id);
    Task AddAsync(Medicamento medicamento);
    Task DeleteAsync(Medicamento medicamento);
    Task UpdateAsync(Medicamento entity);
    Task<IEnumerable<Medicamento>> GetAllByUserIdAsync(int userId); // Manter este
}