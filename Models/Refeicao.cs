// FILE NAME: Refeicao.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace ControleGlicemia.API.Models
{
    public class Refeicao
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; } // Chave estrangeira para o usuário
        public User User { get; set; } = null!; // Propriedade de navegação

        [Required(ErrorMessage = "O campo 'Nome' é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome da refeição não pode exceder 100 caracteres.")]
        public required string Nome { get; set; } // Consistente com DTO

        [StringLength(500, ErrorMessage = "A descrição da refeição não pode exceder 500 caracteres.")]
        public string? Descricao { get; set; } // Pode ser nullable

        [Required(ErrorMessage = "O campo 'DataHora' é obrigatório.")]
        public DateTime DataHora { get; set; }

        [StringLength(500, ErrorMessage = "As observações não podem exceder 500 caracteres.")]
        public string? Observacoes { get; set; } // Pode ser nullable
    }
}