using ERP.Farmacias.Application.DTOs.Security;

namespace ERP.Farmacias.Application.Interfaces.Services.Security;

public interface IAuthService
{
    Task<AuthResultDto> LoginAsync(LoginDto dto);
    Task LogoutAsync();
    Task<UsuarioResponseDto?> GetCurrentUserAsync();
}
