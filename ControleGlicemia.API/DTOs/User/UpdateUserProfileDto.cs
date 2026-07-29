namespace ControleGlicemia.API.DTOs.User;

public class UpdateUserProfileDto
{
    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public double GlicemiaMinima { get; set; } = 70;

    public double GlicemiaMaxima { get; set; } = 140;
}