namespace ERP.Farmacias.Application.DTOs.Security;

public class AuthResultDto
{
    public bool Succeeded { get; set; }
    public bool IsLockedOut { get; set; }
    public string? ErrorMessage { get; set; }
    public UsuarioResponseDto? User { get; set; }
}
