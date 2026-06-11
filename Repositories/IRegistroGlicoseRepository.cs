using ControleGlicemia.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleGlicemia.API.Repositories;

public interface IRegistroGlicoseRepository
{
  
    Task<RegistroGlicose?> GetByIdAsync(int id);
    Task AddAsync(RegistroGlicose registroGlicose);
    Task DeleteAsync(RegistroGlicose registroGlicose);
    Task UpdateAsync(RegistroGlicose entity);
    Task<IEnumerable<RegistroGlicose>> GetAllByUserIdAsync(int userId);
}