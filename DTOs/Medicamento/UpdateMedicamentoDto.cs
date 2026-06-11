using System.ComponentModel.DataAnnotations;

namespace ControleGlicemia.API.DTOs.Medicamento;

public class UpdateMedicamentoDto : CreateMedicamentoDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Id inválido.")]
    public int Id { get; set; }
}