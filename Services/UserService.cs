using ControleGlicemia.API.DTOs.User;
using ControleGlicemia.API.Repositories;
using System.ComponentModel.DataAnnotations;

namespace ControleGlicemia.API.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserProfileDto?> GetProfileAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null) return null;

        return new UserProfileDto
        {
            Id = user.Id,
            Nome = user.Nome,
            Email = user.Email,
            GlicemiaMinima = user.GlicemiaMinima,
            GlicemiaMaxima = user.GlicemiaMaxima,
            CriadoEm = user.CriadoEm
        };
    }

    public async Task<UserProfileDto?> UpdateProfileAsync(int userId, UpdateUserProfileDto updateDto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null) return null;

        var emailNormalizado = (updateDto.Email ?? string.Empty).Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(emailNormalizado))
            throw new ValidationException("O email é obrigatório.");

        if (updateDto.GlicemiaMinima >= updateDto.GlicemiaMaxima)
            throw new ValidationException("A glicemia mínima deve ser menor que a glicemia máxima.");

        var userMesmoEmail = await _userRepository.GetByEmailAsync(emailNormalizado);
        if (userMesmoEmail is not null && userMesmoEmail.Id != userId)
            throw new ValidationException("Este email já está em uso por outro usuário.");

        user.Nome = updateDto.Nome.Trim();
        user.Email = emailNormalizado;
        user.GlicemiaMinima = updateDto.GlicemiaMinima;
        user.GlicemiaMaxima = updateDto.GlicemiaMaxima;

        await _userRepository.UpdateAsync(user);

        return new UserProfileDto
        {
            Id = user.Id,
            Nome = user.Nome,
            Email = user.Email,
            GlicemiaMinima = user.GlicemiaMinima,
            GlicemiaMaxima = user.GlicemiaMaxima,
            CriadoEm = user.CriadoEm
        };
    }
}