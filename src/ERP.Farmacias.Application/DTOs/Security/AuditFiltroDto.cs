namespace ERP.Farmacias.Application.DTOs.Security;

public class AuditFiltroDto
{
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public string? UserId { get; set; }
    public string? Accion { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanoPagina { get; set; } = 20;
}
