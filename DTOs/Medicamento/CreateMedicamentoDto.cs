using System.ComponentModel.DataAnnotations;

namespace ControleGlicemia.API.DTOs.Medicamento;

public class CreateMedicamentoDto : IValidatableObject
{
    [Required(ErrorMessage = "O nome do medicamento é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome do medicamento não pode exceder 100 caracteres.")]
    public required string Nome { get; set; }

    [Required(ErrorMessage = "A dose do medicamento é obrigatória.")]
    [Range(0.1, 1000.0, ErrorMessage = "A dose deve ser entre 0.1 e 1000.")]
    public double Dose { get; set; }

    [Required(ErrorMessage = "A data e hora de tomada são obrigatórias.")]
    public DateTime TomadoEm { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (TomadoEm == default)
        {
            yield return new ValidationResult(
                "A data e hora de tomada são obrigatórias.",
                new[] { nameof(TomadoEm) });
        }

        if (TomadoEm > DateTime.UtcNow.AddMinutes(5))
        {
            yield return new ValidationResult(
                "A data/hora de tomada não pode ser futura.",
                new[] { nameof(TomadoEm) });
        }
    }
}