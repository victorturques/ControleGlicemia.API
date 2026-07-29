using ControleGlicemia.API.Data;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ControleGlicemia.API.Tests.Services;

public class ExpurgoServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Mock<ILogger<ExpurgoService>> _loggerMock;
    private readonly ExpurgoService _service;

    public ExpurgoServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new AppDbContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        var services = new ServiceCollection();
        services.AddSingleton(_context);
        var serviceProvider = services.BuildServiceProvider();
        _scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

        _loggerMock = new Mock<ILogger<ExpurgoService>>();
        _service = new ExpurgoService(_scopeFactory, _loggerMock.Object);
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

    [Fact]
    public async Task ExecutarExpurgoAsync_NaoDeveRemoverRegistrosRecentes()
    {
        var user = CriarUsuario();
        _context.RegistrosGlicose.Add(new RegistroGlicose
        {
            UserId = user.Id,
            Valor = 100,
            MedidoEm = DateTime.UtcNow,
            MomentoMedicao = MomentoMedicao.PreCafe,
            DeletedAt = DateTime.UtcNow.AddDays(-1)
        });
        _context.SaveChanges();

        var method = typeof(ExpurgoService).GetMethod("ExecutarExpurgoAsync",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var task = method?.Invoke(_service, new object[] { CancellationToken.None }) as Task;
        if (task is not null)
            await task;

        var count = await _context.RegistrosGlicose.IgnoreQueryFilters().CountAsync();
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task ExecutarExpurgoAsync_DeveRemover_QuandoDeletedAposRetencao()
    {
        var user = CriarUsuario();
        _context.RegistrosGlicose.Add(new RegistroGlicose
        {
            UserId = user.Id,
            Valor = 100,
            MedidoEm = DateTime.UtcNow.AddDays(-100),
            MomentoMedicao = MomentoMedicao.PreCafe,
            DeletedAt = DateTime.UtcNow.AddDays(-95)
        });
        _context.SaveChanges();

        var method = typeof(ExpurgoService).GetMethod("ExecutarExpurgoAsync",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var task = method?.Invoke(_service, new object[] { CancellationToken.None }) as Task;
        if (task is not null)
            await task;

        var count = await _context.RegistrosGlicose.IgnoreQueryFilters().CountAsync();
        Assert.Equal(0, count);
    }
}
