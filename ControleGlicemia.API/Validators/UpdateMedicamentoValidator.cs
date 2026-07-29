using ControleGlicemia.API.DTOs.Medicamento;
using FluentValidation;

namespace ControleGlicemia.API.Validators;

public class UpdateMedicamentoValidator : AbstractValidator<UpdateMedicamentoDto>
{
    public UpdateMedicamentoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThanOrEqualTo(1).WithMessage("Id inválido.");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome do medicamento é obrigatório.")
            .MaximumLength(100).WithMessage("O nome do medicamento não pode exceder 100 caracteres.");

        RuleFor(x => x.Dose)
            .InclusiveBetween(0.1, 1000.0).WithMessage("A dose deve ser entre 0.1 e 1000.");

        RuleFor(x => x.TomadoEm)
            .NotEmpty().WithMessage("A data e hora de tomada são obrigatórias.")
            .LessThanOrEqualTo(x => DateTime.UtcNow.AddMinutes(5)).WithMessage("A data/hora de tomada não pode ser futura.");
    }
}