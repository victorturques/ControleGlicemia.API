using ControleGlicemia.API.DTOs.User;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Repositories;
using ControleGlicemia.API.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ControleGlicemia.API.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        var loggerMock = new Mock<ILogger<UserService>>();
        _userService = new UserService(_userRepositoryMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task GetProfileAsync_DeveRetornarPerfil_QuandoUsuarioExiste()
    {
        var userId = 1;
        var user = new User
        {
            Id = userId,
            Nome = "Teste",
            Email = "teste@email.com",
            SenhaHash = "hash",
            GlicemiaMinima = 70,
            GlicemiaMaxima = 140,
            CriadoEm = DateTime.UtcNow
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        var result = await _userService.GetProfileAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("Teste", result.Nome);
        Assert.Equal("teste@email.com", result.Email);
    }

    [Fact]
    public async Task GetProfileAsync_DeveRetornarNull_QuandoUsuarioNaoExiste()
    {
        _userRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

        var result = await _userService.GetProfileAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateProfileAsync_DeveAtualizarPerfil_QuandoDadosValidos()
    {
        var userId = 1;
        var user = new User
        {
            Id = userId,
            Nome = "Antigo",
            Email = "antigo@email.com",
            SenhaHash = "hash",
            GlicemiaMinima = 70,
            GlicemiaMaxima = 140,
            CriadoEm = DateTime.UtcNow
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(r => r.GetByEmailAsync("novo@email.com")).ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var updateDto = new UpdateUserProfileDto
        {
            Nome = "Novo",
            Email = "novo@email.com",
            GlicemiaMinima = 80,
            GlicemiaMaxima = 160
        };

        var result = await _userService.UpdateProfileAsync(userId, updateDto);

        Assert.NotNull(result);
        Assert.Equal("Novo", result.Nome);
        Assert.Equal("novo@email.com", result.Email);
        Assert.Equal(80, result.GlicemiaMinima);
        Assert.Equal(160, result.GlicemiaMaxima);
    }

    [Fact]
    public async Task UpdateProfileAsync_DeveLancarExcecao_QuandoGlicemiaMinimaMaiorQueMaxima()
    {
        var userId = 1;
        var user = new User
        {
            Id = userId,
            Nome = "Teste",
            Email = "teste@email.com",
            SenhaHash = "hash",
            GlicemiaMinima = 70,
            GlicemiaMaxima = 140,
            CriadoEm = DateTime.UtcNow
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        var updateDto = new UpdateUserProfileDto
        {
            Nome = "Teste",
            Email = "teste@email.com",
            GlicemiaMinima = 200,
            GlicemiaMaxima = 100
        };

        await Assert.ThrowsAsync<System.ComponentModel.DataAnnotations.ValidationException>(
            () => _userService.UpdateProfileAsync(userId, updateDto));
    }

    [Fact]
    public async Task DeleteAccountAsync_DeveDeletarConta_QuandoUsuarioExiste()
    {
        var userId = 1;
        var user = new User
        {
            Id = userId,
            Nome = "Teste",
            Email = "teste@email.com",
            SenhaHash = "hash",
            CriadoEm = DateTime.UtcNow
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var result = await _userService.DeleteAccountAsync(userId);

        Assert.True(result);
        _userRepositoryMock.Verify(r => r.DeleteAsync(user), Times.Once);
    }

    [Fact]
    public async Task DeleteAccountAsync_DeveRetornarFalse_QuandoUsuarioNaoExiste()
    {
        _userRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

        var result = await _userService.DeleteAccountAsync(99);

        Assert.False(result);
        _userRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateProfileAsync_DeveLancarExcecao_QuandoEmailJaEmUso()
    {
        var userId = 1;
        var user = new User
        {
            Id = userId,
            Nome = "Teste",
            Email = "teste@email.com",
            SenhaHash = "hash",
            GlicemiaMinima = 70,
            GlicemiaMaxima = 140,
            CriadoEm = DateTime.UtcNow
        };

        var outroUser = new User
        {
            Id = 2,
            Nome = "Outro",
            Email = "novo@email.com",
            SenhaHash = "hash"
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(r => r.GetByEmailAsync("novo@email.com")).ReturnsAsync(outroUser);

        var updateDto = new UpdateUserProfileDto
        {
            Nome = "Teste",
            Email = "novo@email.com",
            GlicemiaMinima = 70,
            GlicemiaMaxima = 140
        };

        await Assert.ThrowsAsync<System.ComponentModel.DataAnnotations.ValidationException>(
            () => _userService.UpdateProfileAsync(userId, updateDto));
    }
}
