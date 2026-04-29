using Microsoft.AspNetCore.Identity;

namespace ERP.Farmacias.Domain.Entities.Security;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
