using System.Security.Claims;
using ControleGlicemia.API.Controllers;
using ControleGlicemia.API.DTOs.User;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ControleGlicemia.API.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        var loggerMock = new Mock<ILogger<AuthController>>();
        _controller = new AuthController(_authServiceMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task Register_DeveRetornar201_QuandoCadastroValido()
    {
        var dto = new RegisterDto
        {
            Username = "Novo",
            Email = "novo@email.com",
            Password = "SenhaForte123",
            ConfirmPassword = "SenhaForte123"
        };

        var user = new User { Id = 1, Nome = "Novo", Email = "novo@email.com", SenhaHash = "hash", Role = "User" };
        _authServiceMock.Setup(s => s.RegisterAsync(dto)).ReturnsAsync(user);
        _authServiceMock.Setup(s => s.GenerateJwtToken(user)).Returns("token-jwt");

        var result = await _controller.Register(dto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var response = createdResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task Register_DeveRetornar400_QuandoModelStateInvalido()
    {
        _controller.ModelState.AddModelError("Email", "O email é obrigatório.");
        var dto = new RegisterDto();

        var result = await _controller.Register(dto);

        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = badResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.False(response.Success);
    }

    [Fact]
    public async Task Register_DeveRetornar409_QuandoEmailJaCadastrado()
    {
        var dto = new RegisterDto
        {
            Username = "Novo",
            Email = "existente@email.com",
            Password = "SenhaForte123",
            ConfirmPassword = "SenhaForte123"
        };

        _authServiceMock.Setup(s => s.RegisterAsync(dto)).ReturnsAsync((User?)null);

        var result = await _controller.Register(dto);

        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        var response = conflictResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.False(response.Success);
    }

    [Fact]
    public async Task Login_DeveRetornar200_QuandoCredenciaisValidas()
    {
        var dto = new LoginDto { Email = "teste@email.com", Password = "Senha123" };
        var loginResponse = new LoginResponseDto
        {
            AccessToken = "token",
            RefreshToken = "refresh",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            Role = "User"
        };

        _authServiceMock.Setup(s => s.LoginAsync(dto)).ReturnsAsync(loginResponse);

        var result = await _controller.Login(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task Login_DeveRetornar401_QuandoCredenciaisInvalidas()
    {
        var dto = new LoginDto { Email = "teste@email.com", Password = "Errada" };
        _authServiceMock.Setup(s => s.LoginAsync(dto)).ReturnsAsync((LoginResponseDto?)null);

        var result = await _controller.Login(dto);

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        var response = unauthorizedResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.False(response.Success);
    }

    [Fact]
    public async Task Refresh_DeveRetornar200_QuandoRefreshTokenValido()
    {
        var dto = new RefreshTokenDto { RefreshToken = "refresh-valido" };
        var loginResponse = new LoginResponseDto
        {
            AccessToken = "novo-token",
            RefreshToken = "novo-refresh",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            Role = "User"
        };

        _authServiceMock.Setup(s => s.RefreshTokenAsync("refresh-valido")).ReturnsAsync(loginResponse);

        var result = await _controller.Refresh(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task Refresh_DeveRetornar401_QuandoRefreshTokenInvalido()
    {
        var dto = new RefreshTokenDto { RefreshToken = "invalido" };
        _authServiceMock.Setup(s => s.RefreshTokenAsync("invalido")).ReturnsAsync((LoginResponseDto?)null);

        var result = await _controller.Refresh(dto);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Logout_DeveRetornar200()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim("jti", "test-jti"),
            new Claim("exp", "9999999999")
        }));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        _authServiceMock.Setup(s => s.LogoutAsync(1, "test-jti", It.IsAny<DateTime>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Logout();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.True(response.Success);
    }
}
