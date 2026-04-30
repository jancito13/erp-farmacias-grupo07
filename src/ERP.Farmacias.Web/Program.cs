using ERP.Farmacias.Application.Interfaces.Services.Security;
using ERP.Farmacias.Application.Services.Security;
using ERP.Farmacias.Domain.Entities.Security;
using ERP.Farmacias.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Serilog;
using ERP.Farmacias.Web.Components;

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

// ── Autenticación por Cookie ──────────────────────────────────────
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/acceso-denegado";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// ── MudBlazor ─────────────────────────────────────────────────────
builder.Services.AddMudServices();

// ── Blazor Server ─────────────────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpContextAccessor();

// ── Servicios de Application (registrar aquí por módulo) ──────────
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
// builder.Services.AddScoped<IAuditService, AuditService>();

var app = builder.Build();

// ── Seed de base de datos ─────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();          // Aplica migraciones pendientes
    await DbInitializer.SeedAsync(scope.ServiceProvider);
}

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Error");

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
