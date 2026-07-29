using ControleGlicemia.API.Data;
using ControleGlicemia.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleGlicemia.API.Repositories;

public class TokenBlacklistRepository : ITokenBlacklistRepository
{
    private readonly AppDbContext _context;

    public TokenBlacklistRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsBlacklistedAsync(string tokenJti)
    {
        return await _context.TokenBlacklist
            .AnyAsync(t => t.TokenJti == tokenJti && t.ExpiresAt > DateTime.UtcNow);
    }

    public async Task AddAsync(string tokenJti, DateTime expiresAt)
    {
        _context.TokenBlacklist.Add(new TokenBlacklistEntry
        {
            TokenJti = tokenJti,
            ExpiresAt = expiresAt
        });
        await _context.SaveChangesAsync();
    }

    public async Task CleanupExpiredAsync()
    {
        await _context.TokenBlacklist
            .Where(t => t.ExpiresAt <= DateTime.UtcNow)
            .ExecuteDeleteAsync();
    }
}
