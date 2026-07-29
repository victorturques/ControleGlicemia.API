using System.Security.Claims;
using ControleGlicemia.API.Controllers;
using ControleGlicemia.API.DTOs.User;
using ControleGlicemia.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ControleGlicemia.API.Tests.Controllers;

public class UserControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _authServiceMock = new Mock<IAuthService>();
        var loggerMock = new Mock<ILogger<UserController>>();
        _controller = new UserController(_userServiceMock.Object, _authServiceMock.Object, loggerMock.Object);

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
    }

    [Fact]
    public async Task GetProfile_DeveRetornar200()
    {
        _userServiceMock.Setup(s => s.GetProfileAsync(1)).ReturnsAsync(new UserProfileDto { Id = 1, Nome = "Teste" });

        var result = await _controller.GetProfile();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.True(response!.Success);
    }

    [Fact]
    public async Task GetProfile_DeveRetornar404_QuandoNaoEncontrado()
    {
        _userServiceMock.Setup(s => s.GetProfileAsync(1)).ReturnsAsync((UserProfileDto?)null);

        var result = await _controller.GetProfile();

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task UpdateProfile_DeveRetornar200()
    {
        var dto = new UpdateUserProfileDto { Nome = "Novo", Email = "novo@email.com", GlicemiaMinima = 70, GlicemiaMaxima = 140 };
        _userServiceMock.Setup(s => s.UpdateProfileAsync(1, dto)).ReturnsAsync(new UserProfileDto { Id = 1, Nome = "Novo" });

        var result = await _controller.UpdateProfile(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.True(response!.Success);
    }

    [Fact]
    public async Task DeleteAccount_DeveRetornar200()
    {
        _userServiceMock.Setup(s => s.DeleteAccountAsync(1)).ReturnsAsync(true);
        _authServiceMock.Setup(s => s.LogoutAsync(1, "test-jti", It.IsAny<DateTime>())).Returns(Task.CompletedTask);

        var result = await _controller.DeleteAccount();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value as ApiResponse<object>;
        Assert.NotNull(response);
        Assert.True(response!.Success);
    }

    [Fact]
    public async Task DeleteAccount_DeveRetornar404_QuandoNaoEncontrado()
    {
        _userServiceMock.Setup(s => s.DeleteAccountAsync(1)).ReturnsAsync(false);

        var result = await _controller.DeleteAccount();

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
