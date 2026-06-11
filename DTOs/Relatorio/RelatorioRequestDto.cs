using System.ComponentModel.DataAnnotations;

namespace ControleGlicemia.API.DTOs.Relatorio;

public class RelatorioRequestDto : IValidatableObject
{
    [Required(ErrorMessage = "A data de início é obrigatória.")]
    public DateTime DataInicio { get; set; }

    [Required(ErrorMessage = "A data de fim é obrigatória.")]
    public DateTime DataFim { get; set; }

    [StringLength(120, ErrorMessage = "Nome do médico deve ter no máximo 120 caracteres.")]
    public string? NomeMedico { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DataInicio == default)
        {
            yield return new ValidationResult(
                "A data de início é obrigatória.",
                new[] { nameof(DataInicio) });
        }

        if (DataFim == default)
        {
            yield return new ValidationResult(
                "A data de fim é obrigatória.",
                new[] { nameof(DataFim) });
        }

        if (DataInicio != default && DataFim != default && DataFim < DataInicio)
        {
            yield return new ValidationResult(
                "A data de fim não pode ser menor que a data de início.",
                new[] { nameof(DataFim), nameof(DataInicio) });
        }
    }
}