using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ControleGlicemia.API.Middlewares;
using ControleGlicemia.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace ControleGlicemia.API.Tests.Middlewares;

public class TokenBlacklistMiddlewareTests
{
    private readonly Mock<ITokenBlacklistRepository> _blacklistMock;
    private readonly Mock<ILogger<TokenBlacklistMiddleware>> _loggerMock;
    private readonly TokenBlacklistMiddleware _middleware;

    public TokenBlacklistMiddlewareTests()
    {
        _blacklistMock = new Mock<ITokenBlacklistRepository>();
        _loggerMock = new Mock<ILogger<TokenBlacklistMiddleware>>();

        RequestDelegate next = ctx => Task.CompletedTask;
        _middleware = new TokenBlacklistMiddleware(next);
    }

    private static string GerarJwtComJti(string jti)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MinhaChaveSecretaSuperSeguraDev1234567890!@#$%"));
        var token = new JwtSecurityToken(
            issuer: "Test",
            audience: "Test",
            claims: new[] { new Claim(JwtRegisteredClaimNames.Jti, jti) },
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static HttpContext CriarContextoComToken(string? token = null)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        if (token is not null)
        {
            context.Request.Headers["Authorization"] = $"Bearer {token}";
        }

        return context;
    }

    [Fact]
    public async Task InvokeAsync_DevePassar_QuandoSemToken()
    {
        var context = CriarContextoComToken();

        await _middleware.InvokeAsync(context, _blacklistMock.Object, _loggerMock.Object);

        Assert.Equal(200, context.Response.StatusCode);
        _blacklistMock.Verify(r => r.IsBlacklistedAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_DevePassar_QuandoTokenNaoBlacklistado()
    {
        var tokenJwt = GerarJwtComJti("jti-valido");
        var context = CriarContextoComToken(tokenJwt);
        _blacklistMock.Setup(r => r.IsBlacklistedAsync("jti-valido")).ReturnsAsync(false);

        await _middleware.InvokeAsync(context, _blacklistMock.Object, _loggerMock.Object);

        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_DeveRetornar401_QuandoTokenBlacklistado()
    {
        var tokenJwt = GerarJwtComJti("jti-blacklistado");
        var context = CriarContextoComToken(tokenJwt);
        _blacklistMock.Setup(r => r.IsBlacklistedAsync("jti-blacklistado")).ReturnsAsync(true);

        await _middleware.InvokeAsync(context, _blacklistMock.Object, _loggerMock.Object);

        Assert.Equal(401, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_DeveRetornar401_QuandoTokenInvalido()
    {
        var context = CriarContextoComToken("invalido@@@!!!");

        await _middleware.InvokeAsync(context, _blacklistMock.Object, _loggerMock.Object);

        Assert.Equal(401, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_DevePassar_QuandoHeaderVazio()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await _middleware.InvokeAsync(context, _blacklistMock.Object, _loggerMock.Object);

        Assert.Equal(200, context.Response.StatusCode);
    }
}
