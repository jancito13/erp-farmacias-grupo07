using ERP.Farmacias.Application.Interfaces.Services.Security;
using ERP.Farmacias.Application.Services.Security;
using ERP.Farmacias.Domain.Entities.Security;
using ERP.Farmacias.Infrastructure.Data;
using ERP.Farmacias.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Serilog;
using ERP.Farmacias.Web.Components;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// ── Serilog ──────────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/erp-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

// ── DbContext + SQL Server ─────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly("ERP.Farmacias.Infrastructure")
    ));

// ── ASP.NET Identity ──────────────────────────────────────────────
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ── Antiforgery Cookie ────────────────────────────────────────────
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// ── Autenticación por Cookie ──────────────────────────────────────
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/acceso-denegado";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// ── Políticas de autorización por rol ─────────────────────────────
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministrador",
        p => p.RequireRole("Administrador"));
    options.AddPolicy("RequireCajero",
        p => p.RequireRole("Cajero", "Administrador", "Gerente"));
    options.AddPolicy("RequireContabilidad",
        p => p.RequireRole("Contador", "Administrador", "Gerente"));
    options.AddPolicy("RequireAlmacen",
        p => p.RequireRole("Almacenero", "Administrador", "Gerente"));
    options.AddPolicy("RequireCompras",
        p => p.RequireRole("JefeCompras", "Administrador", "Gerente"));
    options.AddPolicy("RequireRRHH",
        p => p.RequireRole("RRHH", "Administrador", "Gerente"));
    options.AddPolicy("RequireGerencia",
        p => p.RequireRole("Gerente", "Administrador"));
});

// ── MudBlazor ─────────────────────────────────────────────────────
builder.Services.AddMudServices();

// ── Blazor Server ─────────────────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        if (builder.Environment.IsDevelopment())
            options.DetailedErrors = true;
    });
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

// ── Servicios de Application (registrar aquí por módulo) ──────────
builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// ── Seed de base de datos ─────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();          // Aplica migraciones pendientes
    await DbInitializer.SeedAsync(scope.ServiceProvider);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorPages();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
