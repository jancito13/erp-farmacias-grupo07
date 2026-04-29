using ERP.Farmacias.Application.DTOs.Security;

namespace ERP.Farmacias.Application.Interfaces.Services.Security;

public interface IUsuarioService
{
    Task<List<UsuarioResponseDto>> GetAllAsync();
    Task<UsuarioResponseDto?> GetByIdAsync(string id);
    Task<bool> CreateAsync(CrearUsuarioDto dto);
    Task<bool> UpdateAsync(string id, CrearUsuarioDto dto);
    Task<bool> ToggleActiveAsync(string id);
    Task<bool> AssignRoleAsync(AsignarRolDto dto);
}
