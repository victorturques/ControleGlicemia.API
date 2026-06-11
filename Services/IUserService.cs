using ControleGlicemia.API.DTOs.User;

namespace ControleGlicemia.API.Services;

public interface IUserService
{
    Task<UserProfileDto?> GetProfileAsync(int userId);
    Task<UserProfileDto?> UpdateProfileAsync(int userId, UpdateUserProfileDto updateDto);
}