namespace ControleGlicemia.API.Repositories;

public interface ITokenBlacklistRepository
{
    Task<bool> IsBlacklistedAsync(string tokenJti);
    Task AddAsync(string tokenJti, DateTime expiresAt);
    Task CleanupExpiredAsync();
}
