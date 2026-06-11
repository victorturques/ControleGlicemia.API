using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ControleGlicemia.API.DTOs.User;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace ControleGlicemia.API.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
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
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
        };

        await _userRepository.AddAsync(user);
        return user;
    }

    public async Task<string?> LoginAsync(LoginDto loginDto)
    {
        var emailNormalizado = (loginDto.Email ?? string.Empty).Trim().ToLowerInvariant();

        var user = await _userRepository.GetByEmailAsync(emailNormalizado);

        if (user is null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.SenhaHash))
            return null;

        return GenerateJwtToken(user);
    }

    public string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(
            _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured"));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Nome)
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
}