using ControleGlicemia.API.DTOs.Refeicao;
using ControleGlicemia.API.Models;

namespace ControleGlicemia.API.Services;

public interface IRefeicaoService
{
    Task<IEnumerable<RefeicaoDto>> GetAllRefeicoesByUserIdAsync(int userId); // Retorna DTO
    Task<RefeicaoDto?> GetRefeicaoByIdAsync(int id); // Retorna DTO
    Task AddRefeicaoAsync(int userId, CreateRefeicaoDto refeicaoDto); // Usa Create DTO
    Task DeleteRefeicaoAsync(int id);
    Task<RefeicaoDto?> UpdateRefeicaoAsync(int id, int userId, UpdateRefeicaoDto refeicaoDto); // Usa Update DTO e retorna DTO
}