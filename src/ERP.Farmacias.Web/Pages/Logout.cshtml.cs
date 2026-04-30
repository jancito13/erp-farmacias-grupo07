using ERP.Farmacias.Application.Interfaces.Services.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ERP.Farmacias.Web.Pages;

public class LogoutModel(IAuthService authService) : PageModel
{
    public async Task<IActionResult> OnGetAsync()
    {
        await authService.LogoutAsync();
        return Redirect("/login");
    }
}
