using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ================== Services ==================
builder.Services.AddRazorPages();

// Servicios propios
builder.Services.AddScoped<IInventarioService, InventarioService>();

// DbContext de la app (tu ERP)
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// DbContext para Identity (usuarios/roles) -> usa la misma connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity + Roles
builder.Services
    .AddDefaultIdentity<IdentityUser>(o =>
    {
        o.SignIn.RequireConfirmedAccount = false;
        o.Password.RequiredLength = 6;
        o.Password.RequireNonAlphanumeric = false;
        o.Password.RequireUppercase = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Autorización por políticas (módulos)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Ventas", p => p.RequireRole("Admin", "Cajero", "Facturacion"));
    options.AddPolicy("Inventario.Read", p => p.RequireRole("Admin", "Inventario"));
    options.AddPolicy("Inventario.Edit", p => p.RequireRole("Admin")); // solo admin edita
    options.AddPolicy("RRHH", p => p.RequireRole("Admin", "RRHH"));
});


// Autorización por políticas (capacidades)
builder.Services.AddAuthorization(options =>
{
    // ---- CLIENTES ----
    options.AddPolicy("Clientes.Ver", p => p.RequireRole("Admin", "Ventas", "Cajero", "Facturacion"));
    options.AddPolicy("Clientes.Crear", p => p.RequireRole("Admin", "Ventas", "Cajero"));
    options.AddPolicy("Clientes.Editar", p => p.RequireRole("Admin", "Ventas")); // ❗ SIN 'Cajero'
    options.AddPolicy("Clientes.Eliminar", p => p.RequireRole("Admin"));          // ❗ solo Admin

    // (opcional) otras capacidades…
    options.AddPolicy("Ventas", p => p.RequireRole("Admin", "Cajero", "Facturacion"));
    options.AddPolicy("Inventario.Read", p => p.RequireRole("Admin", "Inventario"));
    options.AddPolicy("Inventario.Edit", p => p.RequireRole("Admin"));
    options.AddPolicy("RRHH", p => p.RequireRole("Admin", "RRHH"));
});


// Autorizar por carpetas/páginas 
builder.Services.AddRazorPages(options =>
{
    // Ventas/Facturación
    options.Conventions.AuthorizeFolder("/Ventas", "Ventas");
    options.Conventions.AuthorizeFolder("/Facturacion", "Ventas");

    // Inventario: lectura para Inventario.Read, edición solo Admin
    options.Conventions.AuthorizeFolder("/Inventario", "Inventario.Read");
    options.Conventions.AuthorizePage("/Inventario/EditInv", "Inventario.Edit");
    options.Conventions.AuthorizePage("/Inventario/CreateInv", "Inventario.Edit");

    // RRHH
    options.Conventions.AuthorizeFolder("/RRHH", "RRHH");

    // Login/Logout públicos (UI por defecto de Identity)
    options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/Login");
    options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/Logout");
});

var app = builder.Build();

// ================== Pipeline ==================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// HTTPS + Static
app.UseHttpsRedirection();

// Ruteo
app.UseRouting();

// **Importante**: primero autenticación, luego autorización
app.UseAuthentication();
app.UseAuthorization();

// Static files (tu proyecto ya lo usa)
app.MapStaticAssets();

// Razor Pages
app.MapRazorPages().WithStaticAssets();

// ===== Migraciones + seed (solo en Development) =====
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    // Migrar ambas BD
    var appDb = scope.ServiceProvider.GetRequiredService<AppDBContext>();
    var idDb = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    appDb.Database.Migrate();
    idDb.Database.Migrate();

    // Seed de roles/usuarios demo
    await SeedIdentityAsync(scope.ServiceProvider);
}

app.Run();

// ================== Seed helper ==================
static async Task SeedIdentityAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();

    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string[] roles = { "Admin", "Cajero", "Inventario", "Facturacion", "RRHH" };
    foreach (var r in roles)
        if (!await roleMgr.RoleExistsAsync(r))
            await roleMgr.CreateAsync(new IdentityRole(r));

    // Admin
    var adminEmail = "admin@local";
    var admin = await userMgr.FindByEmailAsync(adminEmail);
    if (admin == null)
    {
        admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        await userMgr.CreateAsync(admin, "Admin123!");
        await userMgr.AddToRoleAsync(admin, "Admin");
    }

    // Cajero demo
    var cajeroEmail = "cajero@local";
    var cajero = await userMgr.FindByEmailAsync(cajeroEmail);
    if (cajero == null)
    {
        cajero = new IdentityUser { UserName = cajeroEmail, Email = cajeroEmail, EmailConfirmed = true };
        await userMgr.CreateAsync(cajero, "Cajero123!");
        await userMgr.AddToRolesAsync(cajero, new[] { "Cajero", "Facturacion" });
    }
}

