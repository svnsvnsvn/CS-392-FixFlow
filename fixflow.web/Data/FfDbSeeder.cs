using fixflow.web.Data;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace fixflow.web.Data
{
    public class FfDbSeeder
    // AMS ***** This class will verify required roles and at least 1 admin exists.  Ensure bare minimum setup in the case of a fresh install or DB
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly FfDbContext _db;
        private readonly ILogger _logger;

        public FfDbSeeder(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, FfDbContext db, ILogger<FfDbSeeder> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _logger = logger;
        }

        public async Task SeedDbAsync()
        {
            bool rolesCreated = true;

            // AMS - Verify Admin role exists, if not create it
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                var status = await _roleManager.CreateAsync(new IdentityRole("Admin"));
                if (!status.Succeeded)
                {
                    rolesCreated = true;
                    foreach (var e in status.Errors)
                        _logger.LogError("Role 'Admin' failed: {Error}", e.Description);
                }
            }

            // AMS - Verify Manager role exists, if not create it
            if (!await _roleManager.RoleExistsAsync("Manager"))
            {
                var status = await _roleManager.CreateAsync(new IdentityRole("Manager"));
                if (!status.Succeeded)
                {
                    rolesCreated = true;
                    foreach (var e in status.Errors)
                        _logger.LogError("Role 'Manager' failed: {Error}", e.Description);
                }
            }

            // AMS - Verify Employee role exists, if not create it
            if (!await _roleManager.RoleExistsAsync("Employee"))
            {
                var status = await _roleManager.CreateAsync(new IdentityRole("Employee"));
                if (!status.Succeeded)
                {
                    rolesCreated = true;
                    foreach (var e in status.Errors)
                        _logger.LogError("Role 'Employee' failed: {Error}", e.Description);
                }
            }

            // AMS - Verify Resident role exists, if not create it
            if (!await _roleManager.RoleExistsAsync("Resident"))
            {
                var status = await _roleManager.CreateAsync(new IdentityRole("Resident"));
                if (!status.Succeeded)
                {
                    rolesCreated = true;
                    foreach (var e in status.Errors)
                        _logger.LogError("Role 'Resident' failed: {Error}", e.Description);
                }
            }

            // AMS - Verify Pending role exists, if not create it
            if (!await _roleManager.RoleExistsAsync("Pending"))
            {
                var status = await _roleManager.CreateAsync(new IdentityRole("Pending"));
                if (!status.Succeeded)
                {
                    rolesCreated = true;
                    foreach (var e in status.Errors)
                        _logger.LogError("Role 'Pending' failed: {Error}", e.Description);
                }
            }

            if (!rolesCreated)
            {
                _logger.LogError("Could not create required user roles. APPLICATION MAY NOT FUNCTION CORRECTLY.");
            }

            // AMS if no admins exist create the default and set to require password change at next login.
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            if (!admins.Any())
            {
                // AMS - Setup new admin user settings

                using var userCreationTransaction = await _db.Database.BeginTransactionAsync();
                try
                {
                    var initialAdmin = new AppUser
                    {
                        UserName = "admin",
                        Email = "admin@fixflow.local",
                        EmailConfirmed = true,
                        ResetPassOnLogin = true
                    };

                    // AMS - Create new admin user
                    string randPassword = $"{Guid.NewGuid():N}".Substring(0, 12) + "aA1!";
                    var resultU = await _userManager.CreateAsync(initialAdmin, randPassword);
                    // AMS - Take success/fail of Admin user creation, assign admin role, and log an appropriate output.
                    if (!resultU.Succeeded)
                    {
                        throw new Exception($"Failed to create initial admin user: {string.Join(", ", resultU.Errors.Select(e => e.Description))}");
                    }

                    var resultUR = await _userManager.AddToRoleAsync(initialAdmin, "Admin");
                    if (!resultUR.Succeeded)
                    {
                        throw new Exception($"Failed to add 'admin' role to admin user: {string.Join(", ", resultUR.Errors.Select(e => e.Description))}");
                    }


                    // Create Profile
                    var initialAdminProfile = new FfUserProfile
                    {
                        FName = "Default",
                        LName = "Administrator",
                        EmployeeId = initialAdmin.Id
                    };

                    _db.FfUserProfiles.Add(initialAdminProfile);
                    await _db.SaveChangesAsync();

                    _logger.LogCritical(@"
                        ==================================================
                            INITIAL ADMIN ACCOUNT CREATED
                            USERNAME: admin
                            PASSWORD: {Password}
                        ==================================================
                        ", randPassword);

                    await userCreationTransaction.CommitAsync();
                }
                catch
                {
                    await userCreationTransaction.RollbackAsync();          // Stop db writes if something failed. Prevent half transactions.
                    throw;
                }
            }
        }
    }
}