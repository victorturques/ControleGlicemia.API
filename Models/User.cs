namespace ControleGlicemia.API.Models;

public class User
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public required string Email { get; set; }
    public required string SenhaHash { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public double GlicemiaMinima { get; set; } = 70;
    public double GlicemiaMaxima { get; set; } = 140;

    // Navigation Properties
    public ICollection<RegistroGlicose> RegistrosGlicose { get; set; } = [];
    public ICollection<Medicamento> Medicamentos { get; set; } = [];
    public ICollection<Refeicao> Refeicoes { get; set; } = [];
    public ICollection<RegistroDiario> RegistrosDiarios { get; set; } = [];
}