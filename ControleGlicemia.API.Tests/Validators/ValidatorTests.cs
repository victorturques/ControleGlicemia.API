using ControleGlicemia.API.DTOs.Medicamento;
using ControleGlicemia.API.DTOs.RegistroGlicose;
using ControleGlicemia.API.DTOs.User;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Validators;
using Xunit;

namespace ControleGlicemia.API.Tests.Validators;

public class ValidatorTests
{
    [Fact]
    public void CreateRegistroGlicoseValidator_DeveSerValido_QuandoDadosCorretos()
    {
        var validator = new CreateRegistroGlicoseValidator();
        var dto = new CreateRegistroGlicoseDto
        {
            Valor = 100,
            MedidoEm = DateTime.UtcNow.AddMinutes(-10),
            MomentoMedicao = MomentoMedicao.PreCafe,
            Observacoes = "Normal"
        };

        var result = validator.Validate(dto);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateRegistroGlicoseValidator_DeveFalhar_QuandoValorForaDoRange()
    {
        var validator = new CreateRegistroGlicoseValidator();
        var dto = new CreateRegistroGlicoseDto
        {
            Valor = 1000,
            MedidoEm = DateTime.UtcNow.AddMinutes(-10),
            MomentoMedicao = MomentoMedicao.PreCafe
        };

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Valor");
    }

    [Fact]
    public void CreateRegistroGlicoseValidator_DeveFalhar_QuandoMedidoEmFuturo()
    {
        var validator = new CreateRegistroGlicoseValidator();
        var dto = new CreateRegistroGlicoseDto
        {
            Valor = 100,
            MedidoEm = DateTime.UtcNow.AddDays(1),
            MomentoMedicao = MomentoMedicao.PreCafe
        };

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "MedidoEm");
    }

    [Fact]
    public void CreateRegistroGlicoseValidator_DeveFalhar_QuandoMomentoMedicaoInvalido()
    {
        var validator = new CreateRegistroGlicoseValidator();
        var dto = new CreateRegistroGlicoseDto
        {
            Valor = 100,
            MedidoEm = DateTime.UtcNow.AddMinutes(-10),
            MomentoMedicao = (MomentoMedicao)99
        };

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "MomentoMedicao");
    }

    [Fact]
    public void CreateRegistroGlicoseValidator_DeveFalhar_QuandoObservacoesExcede300()
    {
        var validator = new CreateRegistroGlicoseValidator();
        var dto = new CreateRegistroGlicoseDto
        {
            Valor = 100,
            MedidoEm = DateTime.UtcNow.AddMinutes(-10),
            MomentoMedicao = MomentoMedicao.PreCafe,
            Observacoes = new string('a', 301)
        };

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Observacoes");
    }


    [Fact]
    public void RegisterDtoValidator_DeveSerValido_QuandoDadosCorretos()
    {
        var validator = new RegisterDtoValidator();
        var dto = new RegisterDto
        {
            Username = "UsuarioTeste",
            Email = "teste@email.com",
            Password = "SenhaForte123",
            ConfirmPassword = "SenhaForte123"
        };

        var result = validator.Validate(dto);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void RegisterDtoValidator_DeveFalhar_QuandoSenhaFraca()
    {
        var validator = new RegisterDtoValidator();
        var dto = new RegisterDto
        {
            Username = "Usuario",
            Email = "teste@email.com",
            Password = "fraca",
            ConfirmPassword = "fraca"
        };

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }

    [Fact]
    public void RegisterDtoValidator_DeveFalhar_QuandoSenhaSemMaiuscula()
    {
        var validator = new RegisterDtoValidator();
        var dto = new RegisterDto
        {
            Username = "Usuario",
            Email = "teste@email.com",
            Password = "senhaforte123",
            ConfirmPassword = "senhaforte123"
        };

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }

    [Fact]
    public void RegisterDtoValidator_DeveFalhar_QuandoSenhaSemNumero()
    {
        var validator = new RegisterDtoValidator();
        var dto = new RegisterDto
        {
            Username = "Usuario",
            Email = "teste@email.com",
            Password = "SenhaForte",
            ConfirmPassword = "SenhaForte"
        };

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }

    [Fact]
    public void RegisterDtoValidator_DeveFalhar_QuandoEmailInvalido()
    {
        var validator = new RegisterDtoValidator();
        var dto = new RegisterDto
        {
            Username = "Usuario",
            Email = "email-invalido",
            Password = "SenhaForte123",
            ConfirmPassword = "SenhaForte123"
        };

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public void RegisterDtoValidator_DeveFalhar_QuandoConfirmPasswordDiferente()
    {
        var validator = new RegisterDtoValidator();
        var dto = new RegisterDto
        {
            Username = "Usuario",
            Email = "teste@email.com",
            Password = "SenhaForte123",
            ConfirmPassword = "SenhaDiferente"
        };

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ConfirmPassword");
    }


    [Fact]
    public void LoginDtoValidator_DeveSerValido_QuandoDadosCorretos()
    {
        var validator = new LoginDtoValidator();
        var dto = new LoginDto
        {
            Email = "teste@email.com",
            Password = "Senha123"
        };

        var result = validator.Validate(dto);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void LoginDtoValidator_DeveFalhar_QuandoEmailVazio()
    {
        var validator = new LoginDtoValidator();
        var dto = new LoginDto
        {
            Email = "",
            Password = "Senha123"
        };

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public void LoginDtoValidator_DeveFalhar_QuandoEmailInvalido()
    {
        var validator = new LoginDtoValidator();
        var dto = new LoginDto
        {
            Email = "invalido",
            Password = "Senha123"
        };

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }


    [Fact]
    public void CreateMedicamentoValidator_DeveSerValido_QuandoDadosCorretos()
    {
        var validator = new CreateMedicamentoValidator();
        var dto = new CreateMedicamentoDto
        {
            Nome = "Insulina",
            Dose = 10.0,
            TomadoEm = DateTime.UtcNow.AddHours(-1)
        };

        var result = validator.Validate(dto);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateMedicamentoValidator_DeveFalhar_QuandoNomeVazio()
    {
        var validator = new CreateMedicamentoValidator();
        var dto = new CreateMedicamentoDto
        {
            Nome = "",
            Dose = 10.0,
            TomadoEm = DateTime.UtcNow.AddHours(-1)
        };

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Nome");
    }

    [Fact]
    public void CreateMedicamentoValidator_DeveFalhar_QuandoDoseExcedeLimite()
    {
        var validator = new CreateMedicamentoValidator();
        var dto = new CreateMedicamentoDto
        {
            Nome = "Insulina",
            Dose = 1001.0,
            TomadoEm = DateTime.UtcNow.AddHours(-1)
        };

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Dose");
    }

    [Fact]
    public void CreateMedicamentoValidator_DeveFalhar_QuandoTomadoEmFuturo()
    {
        var validator = new CreateMedicamentoValidator();
        var dto = new CreateMedicamentoDto
        {
            Nome = "Insulina",
            Dose = 10.0,
            TomadoEm = DateTime.UtcNow.AddDays(1)
        };

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TomadoEm");
    }


    [Fact]
    public void UpdateUserProfileDtoValidator_DeveFalhar_QuandoGlicemiaMinimaMaiorQueMaxima()
    {
        var validator = new UpdateUserProfileDtoValidator();
        var dto = new UpdateUserProfileDto
        {
            Nome = "Teste",
            Email = "teste@email.com",
            GlicemiaMinima = 200,
            GlicemiaMaxima = 100
        };

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void UpdateUserProfileDtoValidator_DeveSerValido_QuandoDadosCorretos()
    {
        var validator = new UpdateUserProfileDtoValidator();
        var dto = new UpdateUserProfileDto
        {
            Nome = "Teste",
            Email = "teste@email.com",
            GlicemiaMinima = 70,
            GlicemiaMaxima = 140
        };

        var result = validator.Validate(dto);

        Assert.True(result.IsValid);
    }


    [Fact]
    public void CreateRefeicaoValidator_DeveFalhar_QuandoNomeVazio()
    {
        var validator = new CreateRefeicaoValidator();
        var dto = new ControleGlicemia.API.DTOs.Refeicao.CreateRefeicaoDto
        {
            Nome = "",
            DataHora = DateTime.UtcNow.AddHours(-1)
        };

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Nome");
    }
}
