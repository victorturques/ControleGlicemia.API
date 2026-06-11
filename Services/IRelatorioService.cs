using ControleGlicemia.API.DTOs.Relatorio;

namespace ControleGlicemia.API.Services;

public interface IRelatorioService
{
    Task<byte[]> GerarRelatorioPdfAsync(int userId, RelatorioRequestDto request);
}