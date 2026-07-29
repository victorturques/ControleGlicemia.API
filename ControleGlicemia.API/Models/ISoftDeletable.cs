namespace ControleGlicemia.API.Models;

public interface ISoftDeletable
{
    DateTime? DeletedAt { get; set; }
}
