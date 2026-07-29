namespace ControleGlicemia.API.DTOs.Medicamento;

public class CreateMedicamentoDto
{
    public required string Nome { get; set; }

    public double Dose { get; set; }

    public DateTime TomadoEm { get; set; }
}