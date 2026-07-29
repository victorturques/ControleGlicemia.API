using System.ComponentModel.DataAnnotations;

namespace ControleGlicemia.API.Models;

public class Medicamento : ISoftDeletable
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required(ErrorMessage = "O nome do medicamento é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome do medicamento não pode exceder 100 caracteres.")]
    public required string Nome { get; set; }

    [Required]
    [Range(0.1, 1000.0, ErrorMessage = "A dose deve ser entre 0.1 e 1000.")]
    public double Dose { get; set; }

    [Required(ErrorMessage = "A data/hora da tomada é obrigatória.")]
    public DateTime TomadoEm { get; set; } = DateTime.UtcNow;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public User User { get; set; } = null!;
    public DateTime? DeletedAt { get; set; }
}
