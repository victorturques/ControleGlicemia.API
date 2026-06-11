namespace ControleGlicemia.API.Models;

public class Medicamento
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Nome { get; set; } // Adicionado 'required'
    public double Dose { get; set; }
    public DateTime TomadoEm { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = null!; // Inicializado
}
