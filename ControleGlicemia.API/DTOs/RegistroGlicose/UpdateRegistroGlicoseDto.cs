using ControleGlicemia.API.Models;

namespace ControleGlicemia.API.DTOs.RegistroGlicose
{
    public class UpdateRegistroGlicoseDto
    {
        public int Id { get; set; }

        public double Valor { get; set; }

        public DateTime MedidoEm { get; set; }

        public MomentoMedicao MomentoMedicao { get; set; }

        public string? Observacoes { get; set; }
    }
}