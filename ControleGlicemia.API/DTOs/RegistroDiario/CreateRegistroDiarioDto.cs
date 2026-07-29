namespace ControleGlicemia.API.DTOs.RegistroDiario;

public class CreateRegistroDiarioDto
{
    public string? Observacoes { get; set; }

    public DateTime Data { get; set; }
}