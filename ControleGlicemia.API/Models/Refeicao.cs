using System.ComponentModel.DataAnnotations;

namespace ControleGlicemia.API.Models;

public class Refeicao : ISoftDeletable
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    public User User { get; set; } = null!;

    [Required(ErrorMessage = "O campo 'Nome' é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome da refeição não pode exceder 100 caracteres.")]
    public required string Nome { get; set; }

    [StringLength(500, ErrorMessage = "A descrição da refeição não pode exceder 500 caracteres.")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "O campo 'DataHora' é obrigatório.")]
    public DateTime DataHora { get; set; }

    [StringLength(500, ErrorMessage = "As observações não podem exceder 500 caracteres.")]
    public string? Observacoes { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public DateTime? DeletedAt { get; set; }
}
