using ControleGlicemia.API.DTOs.Refeicao;
using FluentValidation;

namespace ControleGlicemia.API.Validators;

public class CreateRefeicaoValidator : AbstractValidator<CreateRefeicaoDto>
{
    public CreateRefeicaoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O campo 'Nome' é obrigatório.")
            .MaximumLength(100).WithMessage("O nome da refeição não pode exceder 100 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(500).WithMessage("A descrição da refeição não pode exceder 500 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Descricao));

        RuleFor(x => x.DataHora)
            .NotEmpty().WithMessage("O campo 'DataHora' é obrigatório.")
            .LessThanOrEqualTo(x => DateTime.UtcNow.AddMinutes(5)).WithMessage("A data/hora da refeição não pode ser futura.");

        RuleFor(x => x.Observacoes)
            .MaximumLength(500).WithMessage("As observações não podem exceder 500 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Observacoes));
    }
}