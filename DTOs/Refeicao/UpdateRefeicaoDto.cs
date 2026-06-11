using System.ComponentModel.DataAnnotations;

namespace ControleGlicemia.API.DTOs.Refeicao;

public class UpdateRefeicaoDto : CreateRefeicaoDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Id inválido.")]
    public int Id { get; set; }
}