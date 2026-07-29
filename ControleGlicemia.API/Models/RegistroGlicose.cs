using System.ComponentModel.DataAnnotations;

namespace ControleGlicemia.API.Models;

public class RegistroGlicose : ISoftDeletable
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [Range(1, 999, ErrorMessage = "O valor da glicose deve estar entre 1 e 999.")]
    public double Valor { get; set; }

    [Required(ErrorMessage = "A data da medição é obrigatória.")]
    public DateTime MedidoEm { get; set; } = DateTime.UtcNow;

    [Required(ErrorMessage = "O momento da medição é obrigatório.")]
    public MomentoMedicao MomentoMedicao { get; set; }

    [StringLength(300, ErrorMessage = "Observações devem ter no máximo 300 caracteres.")]
    public string? Observacoes { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public User User { get; set; } = null!;
    public DateTime? DeletedAt { get; set; }
}
