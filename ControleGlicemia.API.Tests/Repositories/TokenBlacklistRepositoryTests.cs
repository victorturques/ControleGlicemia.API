using ControleGlicemia.API.Data;
using ControleGlicemia.API.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ControleGlicemia.API.Tests.Repositories;

public class TokenBlacklistRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly TokenBlacklistRepository _repository;

    public TokenBlacklistRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new AppDbContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        _repository = new TokenBlacklistRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }

    [Fact]
    public async Task IsBlacklistedAsync_DeveRetornarTrue_QuandoTokenNaListaENaoExpirado()
    {
        await _repository.AddAsync("jti-ativo", DateTime.UtcNow.AddHours(1));

        var result = await _repository.IsBlacklistedAsync("jti-ativo");

        Assert.True(result);
    }

    [Fact]
    public async Task IsBlacklistedAsync_DeveRetornarFalse_QuandoTokenExpirado()
    {
        await _repository.AddAsync("jti-expirado", DateTime.UtcNow.AddHours(-1));

        var result = await _repository.IsBlacklistedAsync("jti-expirado");

        Assert.False(result);
    }

    [Fact]
    public async Task IsBlacklistedAsync_DeveRetornarFalse_QuandoTokenNaoExiste()
    {
        var result = await _repository.IsBlacklistedAsync("jti-inexistente");

        Assert.False(result);
    }

    [Fact]
    public async Task CleanupExpiredAsync_DeveRemoverTokensExpirados()
    {
        await _repository.AddAsync("jti-valido", DateTime.UtcNow.AddHours(1));
        await _repository.AddAsync("jti-expirado", DateTime.UtcNow.AddHours(-1));

        await _repository.CleanupExpiredAsync();

        Assert.False(await _repository.IsBlacklistedAsync("jti-expirado"));
        Assert.True(await _repository.IsBlacklistedAsync("jti-valido"));
    }

    [Fact]
    public async Task AddAsync_DeveAdicionarToken_ComDadosCorretos()
    {
        var expiresAt = DateTime.UtcNow.AddHours(2);

        await _repository.AddAsync("jti-novo", expiresAt);

        var blacklisted = await _repository.IsBlacklistedAsync("jti-novo");
        Assert.True(blacklisted);
    }
}
