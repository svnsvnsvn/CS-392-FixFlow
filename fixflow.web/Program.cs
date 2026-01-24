using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using fixflow.web.Data;

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

var app = builder.Build();

// AMS - Apply migrations (for DB init and validation)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FfDbContext>();
    db.Database.Migrate();
}


// AMS - Create a dev user. Delete for production
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    var devUser = new AppUser { UserName = "fixflow", Email = "dev@fixflow.local" };
    if (await userManager.FindByNameAsync(devUser.UserName) == null)
    {
        await userManager.CreateAsync(devUser, "fixflow");
    }
}


app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllers();       // For APIs later

app.MapGet("/", context =>
{
    context.Response.Redirect("/Account/Login");
    return Task.CompletedTask;
});

app.Run();
