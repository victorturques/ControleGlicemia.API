using ControleGlicemia.API.Data;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ControleGlicemia.API.Tests.Repositories;

public class GenericRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly GenericRepository<RegistroGlicose> _repository;

    public GenericRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new AppDbContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        _repository = new GenericRepository<RegistroGlicose>(_context);
    }

    public void Dispose()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }

    private User CriarUsuario()
    {
        var user = new User
        {
            Nome = "Teste",
            Email = $"teste_{Guid.NewGuid()}@email.com",
            SenhaHash = "hash"
        };
        _context.Users.Add(user);
        _context.SaveChanges();
        return user;
    }

    private RegistroGlicose CriarRegistro(int userId)
    {
        var registro = new RegistroGlicose
        {
            UserId = userId,
            Valor = 100,
            MedidoEm = DateTime.UtcNow,
            MomentoMedicao = MomentoMedicao.PreCafe
        };
        _repository.AddAsync(registro).GetAwaiter().GetResult();
        return registro;
    }

    [Fact]
    public async Task AddAsync_DeveAdicionarEntidade()
    {
        var user = CriarUsuario();
        var registro = new RegistroGlicose
        {
            UserId = user.Id,
            Valor = 120,
            MedidoEm = DateTime.UtcNow,
            MomentoMedicao = MomentoMedicao.PosCafe
        };

        await _repository.AddAsync(registro);

        Assert.True(registro.Id > 0);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarEntidade_QuandoExiste()
    {
        var user = CriarUsuario();
        var registro = CriarRegistro(user.Id);

        var result = await _repository.GetByIdAsync(registro.Id);

        Assert.NotNull(result);
        Assert.Equal(registro.Id, result!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarNull_QuandoNaoExiste()
    {
        var result = await _repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_DeveAtualizarEntidade()
    {
        var user = CriarUsuario();
        var registro = CriarRegistro(user.Id);

        registro.Valor = 200;
        await _repository.UpdateAsync(registro);

        var updated = await _repository.GetByIdAsync(registro.Id);
        Assert.NotNull(updated);
        Assert.Equal(200, updated!.Valor);
    }

    [Fact]
    public async Task DeleteAsync_DeveSoftDeletar_QuandoISoftDeletable()
    {
        var user = CriarUsuario();
        var registro = CriarRegistro(user.Id);

        await _repository.DeleteAsync(registro);

        var deleted = await _context.RegistrosGlicose.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.Id == registro.Id);
        Assert.NotNull(deleted);
        Assert.NotNull(deleted.DeletedAt);
    }

    [Fact]
    public async Task GetPagedByUserIdAsync_DeveRetornarPaginado()
    {
        var user = CriarUsuario();
        for (int i = 0; i < 5; i++)
        {
            CriarRegistro(user.Id);
        }

        var result = await _repository.GetPagedByUserIdAsync(user.Id, 1, 3);

        Assert.NotNull(result);
        Assert.Equal(5, result.TotalCount);
        Assert.Equal(3, result.Items.Count());
        Assert.Equal(1, result.Page);
        Assert.Equal(3, result.PageSize);
    }

    [Fact]
    public async Task GetPagedByUserIdAsync_DeveLimitarPageSize()
    {
        var result = await _repository.GetPagedByUserIdAsync(1, 1, 200);

        Assert.Equal(100, result.PageSize);
    }

    [Fact]
    public async Task GetAllAsync_DeveRetornarTodos()
    {
        var user = CriarUsuario();
        CriarRegistro(user.Id);
        CriarRegistro(user.Id);

        var result = await _repository.GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllByUserIdAsync_DeveFiltrarPorUsuario()
    {
        var user1 = CriarUsuario();
        var user2 = CriarUsuario();
        CriarRegistro(user1.Id);
        CriarRegistro(user1.Id);
        CriarRegistro(user2.Id);

        var result = await _repository.GetAllByUserIdAsync(user1.Id);

        Assert.Equal(2, result.Count());
    }
}
