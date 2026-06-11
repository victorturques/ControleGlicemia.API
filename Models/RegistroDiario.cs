namespace ControleGlicemia.API.Models;

public class RegistroDiario
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Observacoes { get; set; }
    public DateTime Data { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = null!;
}