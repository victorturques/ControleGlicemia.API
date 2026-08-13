using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using ControleGlicemia.API.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ControleGlicemia.API.Tests.Middlewares;

public class ExceptionHandlingMiddlewareTests
{
    private static ExceptionHandlingMiddleware CreateMiddleware(RequestDelegate? next = null)
    {
        next ??= _ => throw new Exception("Erro interno inesperado");
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        return new ExceptionHandlingMiddleware(next, loggerMock.Object);
    }

    private static async Task<(int StatusCode, JsonDocument? Body)> ExecuteMiddleware(ExceptionHandlingMiddleware middleware)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        context.Response.Body.Position = 0;
        var statusCode = context.Response.StatusCode;

        JsonDocument? body = null;
        if (context.Response.Body.Length > 0)
        {
            body = await JsonDocument.ParseAsync(context.Response.Body);
        }

        return (statusCode, body);
    }

    [Fact]
    public async Task InvokeAsync_DeveChamarNext_QuandoSemExcecao()
    {
        var invoked = false;
        var middleware = CreateMiddleware(ctx =>
        {
            invoked = true;
            return Task.CompletedTask;
        });

        await middleware.InvokeAsync(new DefaultHttpContext());

        Assert.True(invoked);
    }

    [Fact]
    public async Task InvokeAsync_DeveRetornar400_QuandoValidationException()
    {
        var middleware = CreateMiddleware(_ => throw new ValidationException("Campo inválido"));
        var (statusCode, body) = await ExecuteMiddleware(middleware);

        Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        Assert.NotNull(body);
        Assert.Equal("Erro de validação", body!.RootElement.GetProperty("title").GetString());
        Assert.Equal("Campo inválido", body.RootElement.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task InvokeAsync_DeveRetornar401_QuandoUnauthorizedAccessException()
    {
        var middleware = CreateMiddleware(_ => throw new UnauthorizedAccessException("Sem permissão"));
        var (statusCode, body) = await ExecuteMiddleware(middleware);

        Assert.Equal((int)HttpStatusCode.Unauthorized, statusCode);
        Assert.NotNull(body);
        Assert.Equal("Não autorizado", body!.RootElement.GetProperty("title").GetString());
    }

    [Fact]
    public async Task InvokeAsync_DeveRetornar404_QuandoKeyNotFoundException()
    {
        var middleware = CreateMiddleware(_ => throw new KeyNotFoundException("Recurso x não encontrado"));
        var (statusCode, body) = await ExecuteMiddleware(middleware);

        Assert.Equal((int)HttpStatusCode.NotFound, statusCode);
        Assert.NotNull(body);
        Assert.Equal("Recurso não encontrado", body!.RootElement.GetProperty("title").GetString());
    }

    [Fact]
    public async Task InvokeAsync_DeveRetornar400_QuandoArgumentException()
    {
        var middleware = CreateMiddleware(_ => throw new ArgumentException("Argumento inválido"));
        var (statusCode, body) = await ExecuteMiddleware(middleware);

        Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        Assert.NotNull(body);
        Assert.Equal("Requisição inválida", body!.RootElement.GetProperty("title").GetString());
    }

    [Fact]
    public async Task InvokeAsync_DeveRetornar500_QuandoExceptionGenerica()
    {
        var middleware = CreateMiddleware(_ => throw new InvalidOperationException("Falha no banco"));
        var (statusCode, body) = await ExecuteMiddleware(middleware);

        Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        Assert.NotNull(body);
        Assert.Equal("Erro interno do servidor", body!.RootElement.GetProperty("title").GetString());
    }

    [Fact]
    public async Task InvokeAsync_DeveRetornarProblemJson()
    {
        var middleware = CreateMiddleware(_ => throw new Exception("Erro"));
        var (_, body) = await ExecuteMiddleware(middleware);

        Assert.NotNull(body);
        Assert.True(body!.RootElement.TryGetProperty("type", out _));
        Assert.True(body.RootElement.TryGetProperty("title", out _));
        Assert.True(body.RootElement.TryGetProperty("status", out _));
        Assert.True(body.RootElement.TryGetProperty("detail", out _));
    }
}
