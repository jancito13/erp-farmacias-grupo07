using ERP.Farmacias.Application.DTOs.Security;
using ERP.Farmacias.Application.Interfaces.Services.Security;
using ERP.Farmacias.Domain.Entities.Security;
using Microsoft.AspNetCore.Identity;

namespace ERP.Farmacias.Application.Services.Security;

public class UsuarioService : IUsuarioService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public UsuarioService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<List<UsuarioResponseDto>> GetAllAsync()
    {
        var users = _userManager.Users.ToList();
        var result = new List<UsuarioResponseDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(MapToDto(user, roles.FirstOrDefault() ?? string.Empty));
        }

        return result;
    }

    public async Task<UsuarioResponseDto?> GetByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        return MapToDto(user, roles.FirstOrDefault() ?? string.Empty);
    }

    public async Task<bool> CreateAsync(CrearUsuarioDto dto)
    {
        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing is not null)
            throw new InvalidOperationException($"El email '{dto.Email}' ya está registrado.");

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            IsActive = true,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded) return false;

        if (!string.IsNullOrWhiteSpace(dto.RoleName))
            await _userManager.AddToRoleAsync(user, dto.RoleName);

        return true;
    }

    public async Task<bool> UpdateAsync(string id, CrearUsuarioDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return false;

        user.FullName = dto.FullName;
        user.Email = dto.Email;
        user.UserName = dto.Email;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> ToggleActiveAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return false;

        user.IsActive = !user.IsActive;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> AssignRoleAsync(AsignarRolDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user is null) return false;

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        var result = await _userManager.AddToRoleAsync(user, dto.RoleName);
        return result.Succeeded;
    }

    private static UsuarioResponseDto MapToDto(ApplicationUser user, string roleName) => new()
    {
        Id = user.Id,
        FullName = user.FullName,
        Email = user.Email ?? string.Empty,
        RoleName = roleName,
        IsActive = user.IsActive,
        CreatedAt = user.CreatedAt
    };
}
