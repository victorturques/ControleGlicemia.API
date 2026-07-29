using ControleGlicemia.API.Models;

namespace ControleGlicemia.API.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}
