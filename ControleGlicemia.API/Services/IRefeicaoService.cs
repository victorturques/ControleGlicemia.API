using ControleGlicemia.API.DTOs.Refeicao;
using ControleGlicemia.API.Models;

namespace ControleGlicemia.API.Services;

public interface IRefeicaoService
{
    Task<IEnumerable<RefeicaoDto>> GetAllRefeicoesByUserIdAsync(int userId);
    Task<PagedResult<RefeicaoDto>> GetRefeicoesPagedAsync(int userId, int page, int pageSize);
    Task<RefeicaoDto?> GetRefeicaoByIdAsync(int id, int userId);
    Task AddRefeicaoAsync(int userId, CreateRefeicaoDto refeicaoDto);
    Task<bool> DeleteRefeicaoAsync(int id, int userId);
    Task<RefeicaoDto?> UpdateRefeicaoAsync(int id, int userId, UpdateRefeicaoDto refeicaoDto);
}