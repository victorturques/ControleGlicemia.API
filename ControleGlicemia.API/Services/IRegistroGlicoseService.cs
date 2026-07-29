using ControleGlicemia.API.DTOs.RegistroGlicose;
using ControleGlicemia.API.Models;

namespace ControleGlicemia.API.Services
{
    public interface IRegistroGlicoseService
    {
        Task<IEnumerable<RegistroGlicoseDto>> GetAllRegistrosGlicoseByUserIdAsync(int userId);
        Task<PagedResult<RegistroGlicoseDto>> GetRegistrosGlicosePagedAsync(int userId, int page, int pageSize);
        Task<RegistroGlicoseDto?> GetRegistroGlicoseByIdAsync(int id, int userId);
        Task AddRegistroGlicoseAsync(int userId, CreateRegistroGlicoseDto registroGlicoseDto);
        Task<bool> DeleteRegistroGlicoseAsync(int id, int userId);
        Task<RegistroGlicoseDto?> UpdateRegistroGlicoseAsync(int id, int userId, UpdateRegistroGlicoseDto registroGlicoseDto);
    }
}