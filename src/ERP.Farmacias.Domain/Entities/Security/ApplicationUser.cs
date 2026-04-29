namespace ERP.Farmacias.Domain.Entities.Security;

public class ApplicationUser
{
	public Guid Id { get; set; }
	public string FullName { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public string PasswordHash { get; set; } = string.Empty;
	public bool IsActive { get; set; } = true;
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public string Role { get; set; } = string.Empty;
}