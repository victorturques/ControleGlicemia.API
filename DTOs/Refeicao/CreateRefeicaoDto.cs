using System.ComponentModel.DataAnnotations;

namespace ControleGlicemia.API.DTOs.Refeicao;

public class CreateRefeicaoDto : IValidatableObject
{
    [Required(ErrorMessage = "O campo 'Nome' é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome da refeição não pode exceder 100 caracteres.")]
    public required string Nome { get; set; }

    [StringLength(500, ErrorMessage = "A descrição da refeição não pode exceder 500 caracteres.")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "O campo 'DataHora' é obrigatório.")]
    public DateTime DataHora { get; set; }

    [StringLength(500, ErrorMessage = "As observações não podem exceder 500 caracteres.")]
    public string? Observacoes { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DataHora == default)
        {
            yield return new ValidationResult(
                "O campo 'DataHora' é obrigatório.",
                new[] { nameof(DataHora) });
        }

        if (DataHora > DateTime.UtcNow.AddMinutes(5))
        {
            yield return new ValidationResult(
                "A data/hora da refeição não pode ser futura.",
                new[] { nameof(DataHora) });
        }
    }
}