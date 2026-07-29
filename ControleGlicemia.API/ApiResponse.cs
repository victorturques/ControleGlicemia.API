namespace ControleGlicemia.API;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }

    public static ApiResponse<T> Ok(T data, string? message = null)
        => new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Created(T data, string? message = null)
        => new() { Success = true, Data = data, Message = message ?? "Registro criado com sucesso." };

    public static ApiResponse<T> Fail(string message, List<string>? errors = null)
        => new() { Success = false, Message = message, Errors = errors };

    public static ApiResponse<T> NotFound(string message = "Registro não encontrado.")
        => new() { Success = false, Message = message };
}
