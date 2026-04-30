using ERP.Farmacias.Application.DTOs.Security;
using ERP.Farmacias.Application.Interfaces.Services.Security;
using ERP.Farmacias.Domain.Entities.Security;
using ERP.Farmacias.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace ERP.Farmacias.Application.Services.Security;

public class AuthService(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContextAccessor,
    IAuditoriaService auditoriaService) : IAuthService
{
    private string ObtenerIp() =>
        httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    public async Task<AuthResultDto> LoginAsync(LoginDto dto)
    {
        var result = await signInManager.PasswordSignInAsync(
            dto.Email, dto.Password, isPersistent: false, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            UsuarioResponseDto? userDto = null;

            if (user is not null)
            {
                var roles = await userManager.GetRolesAsync(user);
                userDto = new UsuarioResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email!,
                    RoleName = roles.FirstOrDefault() ?? string.Empty,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                };

                await auditoriaService.RegistrarAsync(
                    user.Id, user.Email!, ActionType.Login, "Seguridad",
                    "Inicio de sesión exitoso", ObtenerIp());
            }

            return new AuthResultDto { Succeeded = true, User = userDto };
        }

        var failedUser = await userManager.FindByEmailAsync(dto.Email);
        if (failedUser is not null)
        {
            await auditoriaService.RegistrarAsync(
                failedUser.Id, failedUser.Email!, ActionType.LoginFailed, "Seguridad",
                result.IsLockedOut ? "Cuenta bloqueada" : "Contraseña incorrecta", ObtenerIp());
        }

        return new AuthResultDto
        {
            Succeeded = false,
            IsLockedOut = result.IsLockedOut,
            ErrorMessage = result.IsLockedOut
                ? "Cuenta bloqueada temporalmente. Intenta en 15 minutos."
                : "Correo o contraseña incorrectos."
        };
    }

    public async Task LogoutAsync()
    {
        var user = await GetCurrentUserAsync();
        await signInManager.SignOutAsync();

        if (user is not null)
        {
            await auditoriaService.RegistrarAsync(
                user.Id, user.Email, ActionType.Logout, "Seguridad",
                "Cierre de sesión", ObtenerIp());
        }
    }

    public async Task<UsuarioResponseDto?> GetCurrentUserAsync()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
            return null;

        var user = await userManager.GetUserAsync(httpContext.User);
        if (user is null) return null;

        var roles = await userManager.GetRolesAsync(user);
        return new UsuarioResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            RoleName = roles.FirstOrDefault() ?? string.Empty,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}
