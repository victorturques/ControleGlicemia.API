using ControleGlicemia.API.Data;
using ControleGlicemia.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleGlicemia.API.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var emailNormalizado = (email ?? string.Empty).Trim().ToLowerInvariant();

        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email.ToLower() == emailNormalizado);
    }
}
