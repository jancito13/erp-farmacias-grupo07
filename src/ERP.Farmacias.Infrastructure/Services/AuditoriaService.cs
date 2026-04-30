using ERP.Farmacias.Application.DTOs.Security;
using ERP.Farmacias.Application.Interfaces.Services.Security;
using ERP.Farmacias.Domain.Entities.Security;
using ERP.Farmacias.Domain.Enums;
using ERP.Farmacias.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Farmacias.Infrastructure.Services;

public class AuditoriaService(AppDbContext db) : IAuditoriaService
{
    public async Task RegistrarAsync(string userId, string userEmail, ActionType accion, string modulo, string? detalle, string ip)
    {
        db.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            UserEmail = userEmail,
            Action = accion.ToString(),
            Module = modulo,
            Details = detalle,
            IpAddress = ip,
            Timestamp = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }

    public async Task<(List<AuditLogResponseDto> Items, int Total)> ObtenerPagedAsync(AuditFiltroDto filtro)
    {
        var query = db.AuditLogs.AsQueryable();

        if (filtro.FechaDesde.HasValue)
            query = query.Where(x => x.Timestamp >= filtro.FechaDesde.Value.ToUniversalTime());

        if (filtro.FechaHasta.HasValue)
            query = query.Where(x => x.Timestamp < filtro.FechaHasta.Value.AddDays(1).ToUniversalTime());

        if (!string.IsNullOrEmpty(filtro.UserId))
            query = query.Where(x => x.UserId == filtro.UserId);

        if (!string.IsNullOrEmpty(filtro.Accion))
            query = query.Where(x => x.Action == filtro.Accion);

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.Timestamp)
            .Skip((filtro.Pagina - 1) * filtro.TamanoPagina)
            .Take(filtro.TamanoPagina)
            .Select(x => new AuditLogResponseDto
            {
                Id = x.Id,
                UserId = x.UserId,
                UserEmail = x.UserEmail,
                Action = x.Action,
                Module = x.Module,
                Details = x.Details,
                IpAddress = x.IpAddress,
                Timestamp = x.Timestamp
            })
            .ToListAsync();

        return (items, total);
    }

    public async Task<List<(string Id, string Email)>> ObtenerUsuariosAsync()
    {
        var datos = await db.AuditLogs
            .Select(x => new { x.UserId, x.UserEmail })
            .Distinct()
            .ToListAsync();

        return datos.Select(x => (x.UserId, x.UserEmail)).ToList();
    }
}
