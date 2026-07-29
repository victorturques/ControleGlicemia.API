using System.ComponentModel.DataAnnotations;

namespace ControleGlicemia.API.Models;

public class User : ISoftDeletable
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
    public required string Nome { get; set; }

    [Required(ErrorMessage = "O email é obrigatório.")]
    [StringLength(255, ErrorMessage = "O email não pode exceder 255 caracteres.")]
    public required string Email { get; set; }

    [Required]
    public required string SenhaHash { get; set; }

    [StringLength(20)]
    public string Role { get; set; } = "User";

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public double GlicemiaMinima { get; set; } = 70;
    public double GlicemiaMaxima { get; set; } = 140;

    public ICollection<RegistroGlicose> RegistrosGlicose { get; set; } = [];
    public ICollection<Medicamento> Medicamentos { get; set; } = [];
    public ICollection<Refeicao> Refeicoes { get; set; } = [];
    public ICollection<RegistroDiario> RegistrosDiarios { get; set; } = [];
    public DateTime? DeletedAt { get; set; }
}
