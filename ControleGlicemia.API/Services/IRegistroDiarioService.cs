using ControleGlicemia.API.DTOs.RegistroDiario;
using ControleGlicemia.API.Models;

namespace ControleGlicemia.API.Services;

public interface IRegistroDiarioService
{
    Task<IEnumerable<RegistroDiarioDto>> GetAllRegistrosDiariosByUserIdAsync(int userId);
    Task<PagedResult<RegistroDiarioDto>> GetRegistrosDiariosPagedAsync(int userId, int page, int pageSize);
    Task<RegistroDiarioDto?> GetRegistroDiarioByIdAsync(int id, int userId);
    Task AddRegistroDiarioAsync(int userId, CreateRegistroDiarioDto registroDto);
    Task<bool> DeleteRegistroDiarioAsync(int id, int userId);
    Task<RegistroDiarioDto?> UpdateRegistroDiarioAsync(int id, int userId, UpdateRegistroDiarioDto registroDiarioDto);
}