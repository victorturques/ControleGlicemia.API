using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace ControleGlicemia.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (statusCode, title) = ex switch
        {
            ValidationException => ((int)HttpStatusCode.BadRequest, "Erro de validação"),
            UnauthorizedAccessException => ((int)HttpStatusCode.Unauthorized, "Não autorizado"),
            KeyNotFoundException => ((int)HttpStatusCode.NotFound, "Recurso não encontrado"),
            ArgumentException => ((int)HttpStatusCode.BadRequest, "Requisição inválida"),
            _ => ((int)HttpStatusCode.InternalServerError, "Erro interno do servidor")
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new
        {
            type = $"https://httpstatuses.com/{statusCode}",
            title,
            status = statusCode,
            detail = ex.Message
        };

        var json = JsonSerializer.Serialize(problem);
        await context.Response.WriteAsync(json);
    }
}