namespace ControleGlicemia.API.DTOs.Refeicao;

public class UpdateRefeicaoDto
{
    public int Id { get; set; }

    public required string Nome { get; set; }

    public string? Descricao { get; set; }

    public DateTime DataHora { get; set; }

    public string? Observacoes { get; set; }
}