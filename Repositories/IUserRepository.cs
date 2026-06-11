using ControleGlicemia.API.Models;

namespace ControleGlicemia.API.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task<User?> GetByIdAsync(int id);
    Task UpdateAsync(User user);
}