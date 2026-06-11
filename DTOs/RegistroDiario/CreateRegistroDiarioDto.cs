using System.ComponentModel.DataAnnotations;

namespace ControleGlicemia.API.DTOs.RegistroDiario;

public class CreateRegistroDiarioDto : IValidatableObject
{
    [StringLength(1000, ErrorMessage = "As observações não podem exceder 1000 caracteres.")]
    public string? Observacoes { get; set; }

    [Required(ErrorMessage = "A data do registro diário é obrigatória.")]
    public DateTime Data { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Data == default)
        {
            yield return new ValidationResult(
                "A data do registro diário é obrigatória.",
                new[] { nameof(Data) });
        }

        if (Data > DateTime.UtcNow.AddMinutes(5))
        {
            yield return new ValidationResult(
                "A data do registro diário não pode ser futura.",
                new[] { nameof(Data) });
        }
    }
}