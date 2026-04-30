using ERP.Farmacias.Domain.Entities.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ERP.Farmacias.Web.Pages;

public class LoginModel(SignInManager<ApplicationUser> signInManager) : PageModel
{
    [BindProperty] public string Email { get; set; } = string.Empty;
    [BindProperty] public string Password { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        var result = await signInManager.PasswordSignInAsync(
            Email, Password, isPersistent: false, lockoutOnFailure: true);

        if (result.Succeeded)
            return LocalRedirect(returnUrl ?? "/");

        if (result.IsLockedOut)
            ErrorMessage = "Cuenta bloqueada temporalmente. Intenta en 15 minutos.";
        else
            ErrorMessage = "Correo o contraseña incorrectos.";

        return Page();
    }
}
