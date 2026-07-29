using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ControleGlicemia.API.DTOs.User;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace ControleGlicemia.API.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenBlacklistRepository _tokenBlacklistRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        ITokenBlacklistRepository tokenBlacklistRepository,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _tokenBlacklistRepository = tokenBlacklistRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<User?> RegisterAsync(RegisterDto registerDto)
    {
        var emailNormalizado = (registerDto.Email ?? string.Empty).Trim().ToLowerInvariant();
        var nomeNormalizado = (registerDto.Username ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(emailNormalizado))
            throw new ArgumentException("Email inválido.");

        if (await _userRepository.GetByEmailAsync(emailNormalizado) is not null)
            return null;

        if (registerDto.Password != registerDto.ConfirmPassword)
            throw new ValidationException("As senhas não conferem.");

        var user = new User
        {
            Nome = nomeNormalizado,
            Email = emailNormalizado,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            Role = "User"
        };

        await _userRepository.AddAsync(user);
        return user;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var emailNormalizado = (loginDto.Email ?? string.Empty).Trim().ToLowerInvariant();

        var user = await _userRepository.GetByEmailAsync(emailNormalizado);

        if (user is null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.SenhaHash))
            return null;

        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        user.RefreshToken = BCrypt.Net.BCrypt.HashPassword(refreshToken);
        user.RefreshTokenExpiry = refreshTokenExpiry;
        await _userRepository.UpdateAsync(user);

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            Role = user.Role
        };
    }

    public async Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken)
    {
        var users = await _userRepository.GetAllAsync(u =>
            u.RefreshToken != null && u.RefreshTokenExpiry > DateTime.UtcNow);

        User? user = null;
        foreach (var u in users)
        {
            if (u.RefreshToken is not null && BCrypt.Net.BCrypt.Verify(refreshToken, u.RefreshToken))
            {
                user = u;
                break;
            }
        }

        if (user is null)
            return null;

        var newAccessToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();
        var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        user.RefreshToken = BCrypt.Net.BCrypt.HashPassword(newRefreshToken);
        user.RefreshTokenExpiry = newRefreshTokenExpiry;
        await _userRepository.UpdateAsync(user);

        return new LoginResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            Role = user.Role
        };
    }

    public async Task LogoutAsync(int userId, string? accessTokenJti, DateTime accessTokenExpiresAt)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return;

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        await _userRepository.UpdateAsync(user);

        if (!string.IsNullOrWhiteSpace(accessTokenJti))
        {
            await _tokenBlacklistRepository.AddAsync(accessTokenJti, accessTokenExpiresAt);
        }
    }

    public string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured"));

        var jti = Guid.NewGuid().ToString();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Nome),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
