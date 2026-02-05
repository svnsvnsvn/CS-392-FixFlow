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

app.UseStaticFiles();

// AOU - Apply migrations (for DB init and validation)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FfDbContext>();
    db.Database.Migrate();
}

// AMS ***** This section verify roles and at least 1 admin exists.  ensure bare minimum setup in the case of a fresh install or DB
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    bool rolesCreated = true;

    // AMS - Verify Admin role exists, if not create it
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        var status = await roleManager.CreateAsync(new IdentityRole("Admin"));
        if (!status.Succeeded)
        {
            rolesCreated = true;
            foreach (var e in status.Errors)
                app.Logger.LogError("Role 'Admin' failed: {Error}", e.Description);
        }
    }

    // AMS - Verify Manager role exists, if not create it
    if (!await roleManager.RoleExistsAsync("Manager"))
    {
        var status = await roleManager.CreateAsync(new IdentityRole("Manager"));
        if (!status.Succeeded)
        {
            rolesCreated = true;
            foreach (var e in status.Errors)
                app.Logger.LogError("Role 'Manager' failed: {Error}", e.Description);
        }
    }

    // AMS - Verify Employee role exists, if not create it
    if (!await roleManager.RoleExistsAsync("Employee"))
    {
        var status = await roleManager.CreateAsync(new IdentityRole("Employee"));
        if (!status.Succeeded)
        {
            rolesCreated = true;
            foreach (var e in status.Errors)
                app.Logger.LogError("Role 'Employee' failed: {Error}", e.Description);
        }
    }

    // AMS - Verify Resident role exists, if not create it
    if (!await roleManager.RoleExistsAsync("Resident"))
    {
        var status = await roleManager.CreateAsync(new IdentityRole("Resident"));
        if (!status.Succeeded)
        {
            rolesCreated = true;
            foreach (var e in status.Errors)
                app.Logger.LogError("Role 'Resident' failed: {Error}", e.Description);
        }
    }

    if (!rolesCreated)
    {
        app.Logger.LogError("Could not create required user roles. APPLICATION MAY NOT FUNCTION CORRECTLY.");
    }

    // AMS if no admins exist create the default and set to require password change at next login.
    var admins = await userManager.GetUsersInRoleAsync("Admin");
    if (!admins.Any())
    {
        // AMS - Setup new admin user settings
        var initialAdmin = new AppUser
        {
            UserName = "admin",
            Email = "admin@psts.local",
            EmailConfirmed = true,
            ResetPassOnLogin = true
        };

        // AMS - Verify admin username is not in use
        if (await userManager.FindByNameAsync(initialAdmin.UserName) == null)
        {
            // AMS - Create new admin user
            string randPassword = $"{Guid.NewGuid():N}".Substring(0, 12) + "aA1!";
            var result = await userManager.CreateAsync(initialAdmin, randPassword);
            // AMS - Take success/fail of Admin user creation, assign admin role, and log an appropriate output.
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create initial admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            else
            {
                await userManager.AddToRoleAsync(initialAdmin, "Admin");
                app.Logger.LogCritical(@"
                    ==================================================
                     INITIAL ADMIN ACCOUNT CREATED
                     USERNAME: admin
                     PASSWORD: {Password}
                    ==================================================
                    ", randPassword);
            }
        }

    }
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
    // AOU - Redirect root to home page or login based on auth status
    context.Response.Redirect("/Index");
    return Task.CompletedTask;
});

app.Run();
