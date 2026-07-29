using System.ComponentModel.DataAnnotations;

namespace ControleGlicemia.API.Models;

public class TokenBlacklistEntry
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(500)]
    public required string TokenJti { get; set; }

    public DateTime BlacklistedAt { get; set; } = DateTime.UtcNow;

    public DateTime ExpiresAt { get; set; }
}
