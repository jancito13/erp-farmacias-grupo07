using ERP.Farmacias.Application.DTOs.Security;
using ERP.Farmacias.Domain.Enums;

namespace ERP.Farmacias.Application.Interfaces.Services.Security;

public interface IAuditoriaService
{
    Task RegistrarAsync(string userId, string userEmail, ActionType accion, string modulo, string? detalle, string ip);
    Task<(List<AuditLogResponseDto> Items, int Total)> ObtenerPagedAsync(AuditFiltroDto filtro);
    Task<List<(string Id, string Email)>> ObtenerUsuariosAsync();
}
