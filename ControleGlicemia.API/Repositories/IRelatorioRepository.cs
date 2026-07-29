using ControleGlicemia.API.Models;

namespace ControleGlicemia.API.Repositories;

public interface IRelatorioRepository
{
    Task<User?> GetUserByIdAsync(int userId);
    Task<List<RegistroGlicose>> GetRegistrosGlicoseByPeriodAsync(int userId, DateTime dataInicio, DateTime dataFim);
    Task<List<Medicamento>> GetMedicamentosByPeriodAsync(int userId, DateTime dataInicio, DateTime dataFim);
    Task<List<RegistroDiario>> GetRegistrosDiariosByPeriodAsync(int userId, DateTime dataInicio, DateTime dataFim);
    Task<List<Refeicao>> GetRefeicoesByPeriodAsync(int userId, DateTime dataInicio, DateTime dataFim);
}
