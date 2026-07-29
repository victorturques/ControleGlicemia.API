namespace ControleGlicemia.API.DTOs.Medicamento;

public class UpdateMedicamentoDto
{
    public int Id { get; set; }

    public required string Nome { get; set; }

    public double Dose { get; set; }

    public DateTime TomadoEm { get; set; }
}