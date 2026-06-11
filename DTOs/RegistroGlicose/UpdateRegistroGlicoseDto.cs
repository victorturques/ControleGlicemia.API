using System.ComponentModel.DataAnnotations;
using ControleGlicemia.API.Models;

namespace ControleGlicemia.API.DTOs.RegistroGlicose
{
    public class UpdateRegistroGlicoseDto : IValidatableObject
    {
        [Required(ErrorMessage = "O campo 'Id' é obrigatório para atualização.")]
        [Range(1, int.MaxValue, ErrorMessage = "Id inválido.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo 'Valor' é obrigatório.")]
        [Range(1, 999, ErrorMessage = "O valor da glicose deve estar entre 1 e 999.")]
        public double Valor { get; set; }

        [Required(ErrorMessage = "O campo 'MedidoEm' é obrigatório.")]
        public DateTime MedidoEm { get; set; }

        [Required(ErrorMessage = "O campo 'MomentoMedicao' é obrigatório.")]
        [EnumDataType(typeof(MomentoMedicao), ErrorMessage = "MomentoMedicao inválido.")]
        public MomentoMedicao MomentoMedicao { get; set; }

        [StringLength(300, ErrorMessage = "Observações devem ter no máximo 300 caracteres.")]
        public string? Observacoes { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MedidoEm == default)
            {
                yield return new ValidationResult(
                    "O campo 'MedidoEm' é obrigatório.",
                    new[] { nameof(MedidoEm) });
            }

            if (MedidoEm > DateTime.UtcNow.AddMinutes(5))
            {
                yield return new ValidationResult(
                    "A data da medição não pode ser futura.",
                    new[] { nameof(MedidoEm) });
            }
        }
    }
}