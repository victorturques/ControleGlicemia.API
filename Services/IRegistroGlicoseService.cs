using ControleGlicemia.API.DTOs.RegistroGlicose;

namespace ControleGlicemia.API.Services // Namespace corrigido para consistência
{
    public interface IRegistroGlicoseService
    {
        // Retorna DTOs, não entidades, para desacoplar o serviço do modelo
        Task<IEnumerable<RegistroGlicoseDto>> GetAllRegistrosGlicoseByUserIdAsync(int userId);
        Task<RegistroGlicoseDto?> GetRegistroGlicoseByIdAsync(int id);

        // Usa DTOs de criação/atualização como parâmetros
        Task AddRegistroGlicoseAsync(int userId, CreateRegistroGlicoseDto registroGlicoseDto);
        Task DeleteRegistroGlicoseAsync(int id);
        Task<RegistroGlicoseDto?> UpdateRegistroGlicoseAsync(int id, int userId, UpdateRegistroGlicoseDto registroGlicoseDto);
    }
}