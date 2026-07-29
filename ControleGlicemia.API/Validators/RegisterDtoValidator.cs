using ControleGlicemia.API.DTOs.User;
using FluentValidation;

namespace ControleGlicemia.API.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("O nome de usuário é obrigatório.")
            .Length(3, 50).WithMessage("O nome de usuário deve ter entre 3 e 50 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório.")
            .EmailAddress().WithMessage("Formato de email inválido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(8).WithMessage("A senha deve ter no mínimo 8 caracteres.")
            .Must(p => p.Any(char.IsUpper)).WithMessage("A senha deve conter pelo menos uma letra maiúscula.")
            .Must(p => p.Any(char.IsDigit)).WithMessage("A senha deve conter pelo menos um número.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("A confirmação de senha é obrigatória.")
            .Equal(x => x.Password).WithMessage("As senhas não conferem.");
    }
}