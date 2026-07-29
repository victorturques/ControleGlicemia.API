using System.Security.Claims;
using ControleGlicemia.API.Extensions;
using Xunit;

namespace ControleGlicemia.API.Tests.Extensions;

public class ClaimsPrincipalExtensionsTests
{
    [Fact]
    public void GetUserId_DeveRetornarId_QuandoClaimNameIdentifierExiste()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "42")
        }));

        var userId = user.GetUserId();

        Assert.Equal(42, userId);
    }

    [Fact]
    public void GetUserId_DeveRetornarId_QuandoClaimSubExiste()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("sub", "99")
        }));

        var userId = user.GetUserId();

        Assert.Equal(99, userId);
    }

    [Fact]
    public void GetUserId_DeveLancarExcecao_QuandoClaimNaoExiste()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity());

        Assert.Throws<UnauthorizedAccessException>(() => user.GetUserId());
    }

    [Fact]
    public void GetUserId_DeveLancarExcecao_QuandoClaimNaoNumerico()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "nao-numerico")
        }));

        Assert.Throws<UnauthorizedAccessException>(() => user.GetUserId());
    }
}
