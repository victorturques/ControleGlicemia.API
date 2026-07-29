using ControleGlicemia.API.DTOs.User;
using FluentValidation;

namespace ControleGlicemia.API.Validators;

public class UpdateUserProfileDtoValidator : AbstractValidator<UpdateUserProfileDto>
{
    public UpdateUserProfileDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(100).WithMessage("O nome não pode exceder 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório.")
            .EmailAddress().WithMessage("Formato de email inválido.")
            .MaximumLength(255).WithMessage("O email não pode exceder 255 caracteres.");

        RuleFor(x => x.GlicemiaMinima)
            .LessThan(x => x.GlicemiaMaxima).WithMessage("A glicemia mínima deve ser menor que a glicemia máxima.");
    }
}