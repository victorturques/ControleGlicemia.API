using ControleGlicemia.API.DTOs.User;
using ControleGlicemia.API.Models;

namespace ControleGlicemia.API.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(RegisterDto registerDto);
    Task<string?> LoginAsync(LoginDto loginDto);
    string GenerateJwtToken(User user);
}