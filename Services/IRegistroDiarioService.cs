using ControleGlicemia.API.DTOs.RegistroDiario;
using ControleGlicemia.API.Models;

namespace ControleGlicemia.API.Services;

public interface IRegistroDiarioService
{
    Task<IEnumerable<RegistroDiarioDto>> GetAllRegistrosDiariosByUserIdAsync(int userId); // Retorna DTO
    Task<RegistroDiarioDto?> GetRegistroDiarioByIdAsync(int id); // Retorna DTO
    Task AddRegistroDiarioAsync(int userId, CreateRegistroDiarioDto registroDto); // Usa Create DTO
    Task DeleteRegistroDiarioAsync(int id);
    Task<RegistroDiarioDto?> UpdateRegistroDiarioAsync(int id, int userId, UpdateRegistroDiarioDto registroDiarioDto); // Usa Update DTO e retorna DTO
}