using Microsoft.AspNetCore.Identity;

namespace ERP.Farmacias.Infrastructure.Identity;

public class ApplicationRole : IdentityRole
{
    public string Description { get; set; } = string.Empty;
}
