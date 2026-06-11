using ControleGlicemia.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleGlicemia.API.Repositories;

public interface IRegistroDiarioRepository
{
    // Task<IEnumerable<RegistroDiario>> GetAllRegistrosDiariosByUserIdAsync(int userId); // Remover esta linha
    Task<RegistroDiario?> GetByIdAsync(int id);
    Task AddAsync(RegistroDiario registroDiario);
    Task DeleteAsync(RegistroDiario registroDiario);
    Task UpdateAsync(RegistroDiario entity);
    Task<IEnumerable<RegistroDiario>> GetAllByUserIdAsync(int userId); // Manter este
}