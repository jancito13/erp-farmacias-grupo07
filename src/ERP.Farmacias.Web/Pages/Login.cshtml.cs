using ERP.Farmacias.Application.DTOs.Security;
using ERP.Farmacias.Application.Interfaces.Services.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ERP.Farmacias.Web.Pages;

public class LoginModel(IAuthService authService) : PageModel
{
    [BindProperty] public string Email { get; set; } = string.Empty;
    [BindProperty] public string Password { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        var result = await authService.LoginAsync(new LoginDto { Email = Email, Password = Password });

        if (result.Succeeded)
            return LocalRedirect(returnUrl ?? "/");

        ErrorMessage = result.ErrorMessage;
        return Page();
    }
}
