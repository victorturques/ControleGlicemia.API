using ControleGlicemia.API.DTOs.Medicamento;
using ControleGlicemia.API.Models;

namespace ControleGlicemia.API.Services;

public interface IMedicamentoService
{
    Task<IEnumerable<MedicamentoDto>> GetAllMedicamentosByUserIdAsync(int userId);
    Task<PagedResult<MedicamentoDto>> GetMedicamentosPagedAsync(int userId, int page, int pageSize);
    Task<MedicamentoDto?> GetMedicamentoByIdAsync(int id, int userId);
    Task AddMedicamentoAsync(int userId, CreateMedicamentoDto medicamentoDto);
    Task<bool> DeleteMedicamentoAsync(int id, int userId);
    Task<MedicamentoDto?> UpdateMedicamentoAsync(int id, int userId, UpdateMedicamentoDto medicamentoDto);
}