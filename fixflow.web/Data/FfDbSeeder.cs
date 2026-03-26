using fixflow.web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

            // Create a default location Code for new users if it does not exist
            var defaultLocationExists = await _db.FfBuildingDirectorys
                .FirstOrDefaultAsync(x => x.LocationName == "Unassigned");

            if (defaultLocationExists == null)
            {
                using var defaultLocationTransaction = await _db.Database.BeginTransactionAsync();
                try
                {
                    var defaultLocation = new FfBuildingDirectory
                    {
                        LocationName = "Unassigned",
                        ComplexName = "Unassigned",
                        BuildingNumber = 0,
                        NumUnits = 0,
                        LocationLat = 0,
                        LocationLon = 0
                    };

                    _db.FfBuildingDirectorys.Add(defaultLocation);
                    await _db.SaveChangesAsync();
                    await defaultLocationTransaction.CommitAsync();
                }
                catch
                {
                    await defaultLocationTransaction.RollbackAsync();
                    throw;
                }
            }


            // Get LocationCode of "Unassigned" building once, used by seeded account profiles.
            var unassignedLocationCode = await _db.FfBuildingDirectorys
                .Where(x => x.LocationName == "Unassigned")
                .Select(x => x.LocationCode)
                .SingleAsync();

            // TicketService.GetNextShortCode requires exactly one active row (partial unique index on SeriesIsActive = true).
            if (!await _db.FfTicketConstructoror.AnyAsync(c => c.SeriesIsActive))
            {
                var existing = await _db.FfTicketConstructoror.OrderBy(c => c.Id).FirstOrDefaultAsync();
                if (existing != null)
                {
                    existing.SeriesIsActive = true;
                    await _db.SaveChangesAsync();
                    _logger.LogInformation("Activated existing FfTicketConstructoror row Id={Id} for short codes.", existing.Id);
                }
                else
                {
                    short nextSeries = 1;
                    if (await _db.FfTicketConstructoror.AnyAsync())
                    {
                        var max = await _db.FfTicketConstructoror.MaxAsync(c => c.TicketSeries);
                        nextSeries = (short)Math.Min(max + 1, short.MaxValue);
                    }

                    _db.FfTicketConstructoror.Add(new FfTicketShortCodeConstructor
                    {
                        TicketPrefix = "FF",
                        SeriesIsActive = true,
                        TicketSeries = nextSeries,
                        LastTicketUsed = 0
                    });
                    await _db.SaveChangesAsync();
                    _logger.LogInformation("Seeded default FfTicketConstructoror series {Series} for short codes.", nextSeries);
                }
            }

            // Ensure one account exists for each active role (excluding Pending), all with password "password".
            await EnsureSeedAccountAsync("admin", "admin@fixflow.local", "Default", "Administrator", "Admin", unassignedLocationCode);
            await EnsureSeedAccountAsync("manager", "manager@fixflow.local", "Default", "Manager", "Manager", unassignedLocationCode);
            await EnsureSeedAccountAsync("employee", "employee@fixflow.local", "Default", "Employee", "Employee", unassignedLocationCode);
            await EnsureSeedAccountAsync("resident", "resident@fixflow.local", "Default", "Resident", "Resident", unassignedLocationCode);
        }

        private async Task EnsureSeedAccountAsync(
            string username,
            string email,
            string firstName,
            string lastName,
            string role,
            int locationCode)
        {
            using var userTransaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    user = new AppUser
                    {
                        UserName = username,
                        Email = email,
                        EmailConfirmed = true,
                        ResetPassOnLogin = false
                    };

                    var createResult = await _userManager.CreateAsync(user, "password");
                    if (!createResult.Succeeded)
                    {
                        throw new Exception($"Failed to create seeded user '{username}': {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    // Keep seeded accounts in a known, login-ready state.
                    user.Email ??= email;
                    user.EmailConfirmed = true;
                    user.ResetPassOnLogin = false;
                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        throw new Exception($"Failed to update seeded user '{username}': {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");
                    }

                    if (await _userManager.HasPasswordAsync(user))
                    {
                        var passwordMatches = await _userManager.CheckPasswordAsync(user, "password");
                        if (!passwordMatches)
                        {
                            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                            var resetResult = await _userManager.ResetPasswordAsync(user, token, "password");
                            if (!resetResult.Succeeded)
                            {
                                throw new Exception($"Failed to reset password for seeded user '{username}': {string.Join(", ", resetResult.Errors.Select(e => e.Description))}");
                            }
                        }
                    }
                    else
                    {
                        var addPasswordResult = await _userManager.AddPasswordAsync(user, "password");
                        if (!addPasswordResult.Succeeded)
                        {
                            throw new Exception($"Failed to set password for seeded user '{username}': {string.Join(", ", addPasswordResult.Errors.Select(e => e.Description))}");
                        }
                    }
                }

                if (!await _userManager.IsInRoleAsync(user, role))
                {
                    var addToRoleResult = await _userManager.AddToRoleAsync(user, role);
                    if (!addToRoleResult.Succeeded)
                    {
                        throw new Exception($"Failed to add role '{role}' to user '{username}': {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
                    }
                }

                var profile = await _db.FfUserProfiles.FirstOrDefaultAsync(p => p.FfUserId == user.Id);
                if (profile == null)
                {
                    profile = new FfUserProfile
                    {
                        FfUserId = user.Id,
                        FName = firstName,
                        LName = lastName,
                        LocationCode = locationCode
                    };

                    _db.FfUserProfiles.Add(profile);
                }
                else
                {
                    profile.FName = firstName;
                    profile.LName = lastName;
                    profile.LocationCode = locationCode;
                }

                await _db.SaveChangesAsync();
                await userTransaction.CommitAsync();
            }
            catch
            {
                await userTransaction.RollbackAsync();
                throw;
            }
        }
    }
}