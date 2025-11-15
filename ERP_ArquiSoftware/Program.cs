using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ================== Services ==================
builder.Services.AddRazorPages();

// Servicios propios
builder.Services.AddScoped<IInventarioService, InventarioService>();
builder.Services.AddScoped<INominaService, NominaService>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();



// DbContext de la app (tu ERP)
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// DbContext para Identity (usuarios/roles) -> misma conexión
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

// -------- AUTORIZACIÓN (un solo bloque) --------
builder.Services.AddAuthorization(options =>
{
    // ===== MÓDULOS =====
    options.AddPolicy("Ventas", p => p.RequireRole("Admin", "Cajero", "Facturacion"));
    options.AddPolicy("Inventario.Read", p => p.RequireRole("Admin", "Inventario"));
    options.AddPolicy("Inventario.Edit", p => p.RequireRole("Admin")); // solo Admin edita
    options.AddPolicy("RRHH", p => p.RequireRole("Admin", "RRHH"));

    // ===== CAPACIDADES: CLIENTES =====
    options.AddPolicy("Clientes.Ver", p => p.RequireRole("Admin", "Cajero", "Facturacion"));
    options.AddPolicy("Clientes.Crear", p => p.RequireRole("Admin", "Cajero"));
    options.AddPolicy("Clientes.Editar", p => p.RequireRole("Admin"));
    options.AddPolicy("Clientes.Eliminar", p => p.RequireRole("Admin"));

    // ===== CAPACIDADES: PEDIDOS =====
    options.AddPolicy("Pedidos.Ver", p => p.RequireRole("Admin", "Cajero", "Facturacion"));
    options.AddPolicy("Pedidos.Crear", p => p.RequireRole("Admin", "Cajero"));
    options.AddPolicy("Pedidos.Editar", p => p.RequireRole("Admin"));
    options.AddPolicy("Pedidos.Eliminar", p => p.RequireRole("Admin"));
});

// Autorizar por carpetas/páginas 
builder.Services.AddRazorPages(options =>
{
    // Ventas (clientes, pedidos)
    options.Conventions.AuthorizeFolder("/Ventas", "Ventas");

    // Clientes (granular si quieres asegurar aún más)
    options.Conventions.AuthorizePage("/Ventas/Clientes/Index", "Clientes.Ver");
    options.Conventions.AuthorizePage("/Ventas/Clientes/Upsert", "Clientes.Crear"); // la misma página sirve crear/editar; el botón Editar lo ocultas en vista para roles sin permiso

    // Pedidos (granular)
    options.Conventions.AuthorizePage("/Ventas/Pedidos/Index", "Pedidos.Ver");
    options.Conventions.AuthorizePage("/Ventas/Pedidos/Create", "Pedidos.Crear");
    options.Conventions.AuthorizePage("/Ventas/Pedidos/Edit", "Pedidos.Editar");
    options.Conventions.AuthorizePage("/Ventas/Pedidos/Delete", "Pedidos.Eliminar");

    // Facturación bajo Ventas
    options.Conventions.AuthorizeFolder("/Facturacion", "Ventas");

    // Inventario
    options.Conventions.AuthorizeFolder("/Inventario", "Inventario.Read");
    options.Conventions.AuthorizePage("/Inventario/EditInv", "Inventario.Edit");
    options.Conventions.AuthorizePage("/Inventario/CreateInv", "Inventario.Edit");

    // RRHH
    options.Conventions.AuthorizeFolder("/RRHH", "RRHH");

    // Login/Logout públicos (Identity UI)
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

app.UseHttpsRedirection();

app.UseRouting();

// Autenticación → Autorización (orden importa)
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

// ===== Migraciones + seed (solo en Development) =====
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var appDb = scope.ServiceProvider.GetRequiredService<AppDBContext>();
    var idDb = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    appDb.Database.Migrate();
    idDb.Database.Migrate();

    await SeedIdentityAsync(scope.ServiceProvider);
}

app.Run();

// ================== Seed helper ==================
static async Task SeedIdentityAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();

    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Crea los roles si no existen
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
        //await userMgr.CreateAsync(admin, "Admin123!");
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
