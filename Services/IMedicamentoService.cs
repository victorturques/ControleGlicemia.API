using ControleGlicemia.API.DTOs.Medicamento;
using ControleGlicemia.API.Models; 

namespace ControleGlicemia.API.Services;

public interface IMedicamentoService
{
    Task<IEnumerable<MedicamentoDto>> GetAllMedicamentosByUserIdAsync(int userId); // Retorna DTO
    Task<MedicamentoDto?> GetMedicamentoByIdAsync(int id); // Retorna DTO
    Task AddMedicamentoAsync(int userId, CreateMedicamentoDto medicamentoDto); // Usa Create DTO
    Task DeleteMedicamentoAsync(int id);
    Task<MedicamentoDto?> UpdateMedicamentoAsync(int id, int userId, UpdateMedicamentoDto medicamentoDto); // Usa Update DTO e retorna DTO
}