namespace ControleGlicemia.API.Models;

public class RegistroGlicose
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public double Valor { get; set; }
    public DateTime MedidoEm { get; set; } = DateTime.UtcNow;
    public MomentoMedicao MomentoMedicao { get; set; }
    public string? Observacoes { get; set; }
    public User User { get; set; } = null!;
}