using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using fixflow.web.Data;
using fixflow.web.Domain.Enums;
using fixflow.web.Services;

var builder = WebApplication.CreateBuilder(args);

// AMS - Reqister EF Core
builder.Services.AddDbContext<FfDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<FfDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
});
builder.Services.Configure<IdentityRole>(options =>
{
    options.Name = null; // just ensure EF Core doesn't try weird constraints
});

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddRazorPages();

builder.Services.AddControllers();      // For APIs later

builder.Services.AddScoped<FfDbSeeder>();

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ITicketService, TicketService>();

var app = builder.Build();

app.UseStaticFiles();

// Custom 404 page for missing routes
app.UseStatusCodePagesWithReExecute("/NotFound");

// Apply migrations (for DB init and validation, then run seeder)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FfDbContext>();
    db.Database.Migrate();

    // AMS - Seed Database
    var seeder = scope.ServiceProvider.GetRequiredService<FfDbSeeder>();
    await seeder.SeedDbAsync();
}

app.UseAuthentication();
app.UseAuthorization();

// Residents: /Tickets/List -> /Dashboard (My Tickets). All other roles: /Dashboard -> /Tickets/List (single ticket hub).
app.Use(async (context, next) =>
{
    if (!HttpMethods.IsGet(context.Request.Method)
        || context.User.Identity?.IsAuthenticated != true)
    {
        await next();
        return;
    }

    var path = context.Request.Path.Value ?? "";

    if (string.Equals(path, "/Tickets/List", StringComparison.OrdinalIgnoreCase)
        && (context.User.IsInRole(RoleTypes.Resident.ToString())
            || context.User.IsInRole(RoleTypes.Pending.ToString())))
    {
        context.Response.Redirect("/Dashboard");
        return;
    }

    if (string.Equals(path, "/Dashboard", StringComparison.OrdinalIgnoreCase)
        && (context.User.IsInRole(RoleTypes.Admin.ToString())
            || context.User.IsInRole(RoleTypes.Manager.ToString())
            || context.User.IsInRole(RoleTypes.Employee.ToString())))
    {
        context.Response.Redirect("/Tickets/List");
        return;
    }

    await next();
});

app.MapRazorPages();

app.MapControllers();       // For APIs later

app.MapGet("/", context =>
{
    context.Response.Redirect("/Account/Login");
    return Task.CompletedTask;
});

app.MapGet("/Index", context =>
{
    context.Response.Redirect("/Account/Login");
    return Task.CompletedTask;
});

app.MapGet("/Home", context =>
{
    context.Response.Redirect("/Account/Login");
    return Task.CompletedTask;
});

app.Run();
