using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using ControleGlicemia.API.DTOs.User;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Repositories;
using ControleGlicemia.API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ControleGlicemia.API.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ITokenBlacklistRepository> _blacklistRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _blacklistRepositoryMock = new Mock<ITokenBlacklistRepository>();
        _configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<AuthService>>();

        var keySectionMock = new Mock<IConfigurationSection>();
        keySectionMock.Setup(s => s.Value).Returns("MinhaChaveSecretaSuperSeguraDev1234567890!@#$%");
        _configurationMock.Setup(c => c["Jwt:Key"]).Returns("MinhaChaveSecretaSuperSeguraDev1234567890!@#$%");
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("ControleGlicemiaAPI");
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("ControleGlicemiaApp");

        _authService = new AuthService(
            _userRepositoryMock.Object,
            _blacklistRepositoryMock.Object,
            _configurationMock.Object,
            loggerMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_DeveRegistrarUsuario_QuandoDadosValidos()
    {
        var dto = new RegisterDto
        {
            Username = "NovoUsuario",
            Email = "novo@email.com",
            Password = "SenhaForte123",
            ConfirmPassword = "SenhaForte123"
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync("novo@email.com"))
            .ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        var result = await _authService.RegisterAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("NovoUsuario", result.Nome);
        Assert.Equal("novo@email.com", result.Email);
        Assert.Equal("User", result.Role);
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_DeveRetornarNull_QuandoEmailJaCadastrado()
    {
        var dto = new RegisterDto
        {
            Username = "NovoUsuario",
            Email = "existente@email.com",
            Password = "SenhaForte123",
            ConfirmPassword = "SenhaForte123"
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync("existente@email.com"))
            .ReturnsAsync(new User
            {
                Id = 1,
                Nome = "Existente",
                Email = "existente@email.com",
                SenhaHash = "hash"
            });

        var result = await _authService.RegisterAsync(dto);

        Assert.Null(result);
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_DeveLancarExcecao_QuandoSenhasNaoConferem()
    {
        var dto = new RegisterDto
        {
            Username = "NovoUsuario",
            Email = "novo@email.com",
            Password = "SenhaForte123",
            ConfirmPassword = "SenhaDiferente456"
        };

        await Assert.ThrowsAsync<ValidationException>(
            () => _authService.RegisterAsync(dto));
    }

    [Fact]
    public async Task LoginAsync_DeveRetornarToken_QuandoCredenciaisValidas()
    {
        var dto = new LoginDto
        {
            Email = "teste@email.com",
            Password = "SenhaForte123"
        };

        var user = new User
        {
            Id = 1,
            Nome = "Teste",
            Email = "teste@email.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("SenhaForte123"),
            Role = "User"
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync("teste@email.com"))
            .ReturnsAsync(user);
        _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        var result = await _authService.LoginAsync(dto);

        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
        Assert.Equal("User", result.Role);
    }

    [Fact]
    public async Task LoginAsync_DeveRetornarNull_QuandoCredenciaisInvalidas()
    {
        var dto = new LoginDto
        {
            Email = "teste@email.com",
            Password = "SenhaErrada"
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync("teste@email.com"))
            .ReturnsAsync((User?)null);

        var result = await _authService.LoginAsync(dto);

        Assert.Null(result);
    }

    [Fact]
    public async Task RefreshTokenAsync_DeveRetornarNovoToken_QuandoRefreshTokenValido()
    {
        var refreshToken = "refresh-token-valido";
        var user = new User
        {
            Id = 1,
            Nome = "Teste",
            Email = "teste@email.com",
            SenhaHash = "hash",
            RefreshToken = BCrypt.Net.BCrypt.HashPassword(refreshToken),
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(1),
            Role = "User"
        };

        _userRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(new[] { user });
        _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        var result = await _authService.RefreshTokenAsync(refreshToken);

        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
        Assert.Equal("User", result.Role);
    }

    [Fact]
    public async Task RefreshTokenAsync_DeveRetornarNull_QuandoRefreshTokenInvalido()
    {
        _userRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(Array.Empty<User>());

        var result = await _authService.RefreshTokenAsync("token-invalido");

        Assert.Null(result);
    }

    [Fact]
    public async Task LogoutAsync_DeveLimparRefreshTokenEBlacklistarJti()
    {
        var userId = 1;
        var user = new User
        {
            Id = userId,
            Nome = "Teste",
            Email = "teste@email.com",
            SenhaHash = "hash",
            RefreshToken = "refresh-token",
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(1)
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        _blacklistRepositoryMock.Setup(r => r.AddAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
            .Returns(Task.CompletedTask);

        var jti = "jti-test";
        var expiresAt = DateTime.UtcNow.AddHours(1);

        await _authService.LogoutAsync(userId, jti, expiresAt);

        Assert.Null(user.RefreshToken);
        Assert.Null(user.RefreshTokenExpiry);
        _blacklistRepositoryMock.Verify(r => r.AddAsync(jti, expiresAt), Times.Once);
    }

    [Fact]
    public async Task LogoutAsync_NaoDeveBlacklistar_QuandoJtiVazio()
    {
        var userId = 1;
        var user = new User
        {
            Id = userId,
            Nome = "Teste",
            Email = "teste@email.com",
            SenhaHash = "hash",
            RefreshToken = "refresh-token",
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(1)
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        await _authService.LogoutAsync(userId, null, DateTime.UtcNow);

        _blacklistRepositoryMock.Verify(r => r.AddAsync(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_DeveLancarExcecao_QuandoEmailVazio()
    {
        var dto = new RegisterDto
        {
            Username = "Teste",
            Email = "   ",
            Password = "SenhaForte123",
            ConfirmPassword = "SenhaForte123"
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => _authService.RegisterAsync(dto));
    }

    [Fact]
    public async Task LogoutAsync_NaoDeveFazerNada_QuandoUsuarioNaoExiste()
    {
        _userRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((User?)null);

        await _authService.LogoutAsync(999, "jti-teste", DateTime.UtcNow.AddHours(1));

        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        _blacklistRepositoryMock.Verify(r => r.AddAsync(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
    }

    [Fact]
    public void GenerateJwtToken_DeveConterClaimsCorretas()
    {
        var user = new User
        {
            Id = 1,
            Nome = "Teste",
            Email = "teste@email.com",
            SenhaHash = "hash",
            Role = "User"
        };

        var token = _authService.GenerateJwtToken(user);

        Assert.NotEmpty(token);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.Equal("ControleGlicemiaAPI", jwtToken.Issuer);
        Assert.Equal("ControleGlicemiaApp", jwtToken.Audiences.First());
        Assert.Contains(jwtToken.Claims, c => c.Type == "nameid" && c.Value == "1");
        Assert.Contains(jwtToken.Claims, c => c.Type == "email" && c.Value == "teste@email.com");
        Assert.Contains(jwtToken.Claims, c => c.Type == "unique_name" && c.Value == "Teste");
        Assert.Contains(jwtToken.Claims, c => c.Type == "role" && c.Value == "User");
        Assert.Contains(jwtToken.Claims, c => c.Type == JwtRegisteredClaimNames.Jti);
    }
}
