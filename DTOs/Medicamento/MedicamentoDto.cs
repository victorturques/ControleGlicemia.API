namespace ControleGlicemia.API.DTOs.Medicamento;

public class MedicamentoDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public double Dose { get; set; }
    public DateTime TomadoEm { get; set; }
}