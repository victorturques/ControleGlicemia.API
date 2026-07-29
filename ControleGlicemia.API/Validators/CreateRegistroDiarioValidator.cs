using ControleGlicemia.API.DTOs.RegistroDiario;
using FluentValidation;

namespace ControleGlicemia.API.Validators;

public class CreateRegistroDiarioValidator : AbstractValidator<CreateRegistroDiarioDto>
{
    public CreateRegistroDiarioValidator()
    {
        RuleFor(x => x.Data)
            .NotEmpty().WithMessage("A data do registro diário é obrigatória.")
            .LessThanOrEqualTo(x => DateTime.UtcNow.AddMinutes(5)).WithMessage("A data do registro diário não pode ser futura.");

        RuleFor(x => x.Observacoes)
            .MaximumLength(1000).WithMessage("As observações não podem exceder 1000 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Observacoes));
    }
}