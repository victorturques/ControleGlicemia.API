using System.ComponentModel.DataAnnotations;

namespace ControleGlicemia.API.Models;

public class RegistroDiario : ISoftDeletable
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required(ErrorMessage = "A data do registro diário é obrigatória.")]
    public DateTime Data { get; set; } = DateTime.UtcNow;

    [StringLength(1000, ErrorMessage = "As observações não podem exceder 1000 caracteres.")]
    public string? Observacoes { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public User User { get; set; } = null!;
    public DateTime? DeletedAt { get; set; }
}
