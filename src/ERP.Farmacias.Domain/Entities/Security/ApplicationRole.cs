using Microsoft.AspNetCore.Identity;

namespace ERP.Farmacias.Domain.Entities.Security;

public class ApplicationRole : IdentityRole
{
    public string Description { get; set; } = string.Empty;
}
