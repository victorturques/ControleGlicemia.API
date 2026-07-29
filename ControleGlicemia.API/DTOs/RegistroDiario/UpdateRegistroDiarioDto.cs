using System.ComponentModel.DataAnnotations;

namespace ControleGlicemia.API.DTOs.RegistroDiario;

public class UpdateRegistroDiarioDto : CreateRegistroDiarioDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Id inválido.")]
    public int Id { get; set; }
}