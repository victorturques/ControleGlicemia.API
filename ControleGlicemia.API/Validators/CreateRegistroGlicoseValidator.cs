using ControleGlicemia.API.DTOs.RegistroGlicose;
using ControleGlicemia.API.Models;
using FluentValidation;

namespace ControleGlicemia.API.Validators;

public class CreateRegistroGlicoseValidator : AbstractValidator<CreateRegistroGlicoseDto>
{
    public CreateRegistroGlicoseValidator()
    {
        RuleFor(x => x.Valor)
            .InclusiveBetween(1, 999).WithMessage("O valor da glicose deve estar entre 1 e 999.");

        RuleFor(x => x.MedidoEm)
            .NotEmpty().WithMessage("A data da medição é obrigatória.")
            .LessThanOrEqualTo(x => DateTime.UtcNow.AddMinutes(5)).WithMessage("A data da medição não pode ser futura.");

        RuleFor(x => x.MomentoMedicao)
            .IsInEnum().WithMessage("MomentoMedicao inválido.");

        RuleFor(x => x.Observacoes)
            .MaximumLength(300).WithMessage("Observações devem ter no máximo 300 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Observacoes));
    }
}