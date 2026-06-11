namespace ControleGlicemia.API.DTOs.Refeicao
{
    public class RefeicaoDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Nome { get; set; } = string.Empty; 
        public string? Descricao { get; set; }
        public DateTime DataHora { get; set; }
        public string? Observacoes { get; set; }
    }
}