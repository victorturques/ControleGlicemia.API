namespace ControleGlicemia.API.DTOs.RegistroDiario;

public class RegistroDiarioDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Observacoes { get; set; }
    public DateTime Data { get; set; }
}