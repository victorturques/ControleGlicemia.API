using ControleGlicemia.API.DTOs.User;
using ControleGlicemia.API.Models;

namespace ControleGlicemia.API.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(RegisterDto registerDto);
    Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
    Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(int userId, string? accessTokenJti, DateTime accessTokenExpiresAt);
    string GenerateJwtToken(User user);
}
