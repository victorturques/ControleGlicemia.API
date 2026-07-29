using System.IdentityModel.Tokens.Jwt;
using ControleGlicemia.API.Repositories;
using Microsoft.Extensions.Logging;

namespace ControleGlicemia.API.Middlewares;

public class TokenBlacklistMiddleware
{
    private readonly RequestDelegate _next;

    public TokenBlacklistMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITokenBlacklistRepository blacklistRepository, ILogger<TokenBlacklistMiddleware> logger)
    {
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();

        if (authHeader is not null && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var tokenString = authHeader["Bearer ".Length..].Trim();

            if (!string.IsNullOrWhiteSpace(tokenString))
            {
                try
                {
                    var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);
                    var jti = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

                    if (!string.IsNullOrWhiteSpace(jti))
                    {
                        var isBlacklisted = await blacklistRepository.IsBlacklistedAsync(jti);
                        if (isBlacklisted)
                        {
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync(
                                "{\"success\":false,\"message\":\"Token revogado. Faça login novamente.\"}");
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Erro ao validar blacklist do token");

                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(
                        "{\"success\":false,\"message\":\"Erro ao validar token.\"}");
                    return;
                }
            }
        }

        await _next(context);
    }
}
