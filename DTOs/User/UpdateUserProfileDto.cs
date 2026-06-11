using System.ComponentModel.DataAnnotations;

namespace ControleGlicemia.API.DTOs.User;

public class UpdateUserProfileDto : IValidatableObject
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de email inválido.")]
    public string Email { get; set; } = string.Empty;

    [Range(0.1, 999.9, ErrorMessage = "A glicemia mínima deve estar entre 0.1 e 999.9.")]
    public double GlicemiaMinima { get; set; } = 70;

    [Range(0.1, 999.9, ErrorMessage = "A glicemia máxima deve estar entre 0.1 e 999.9.")]
    public double GlicemiaMaxima { get; set; } = 140;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (GlicemiaMinima >= GlicemiaMaxima)
        {
            yield return new ValidationResult(
                "A glicemia mínima deve ser menor que a glicemia máxima.",
                new[] { nameof(GlicemiaMinima), nameof(GlicemiaMaxima) });
        }
    }
}