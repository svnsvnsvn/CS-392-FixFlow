using fixflow.web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

                    // Get LocationCode of "Unassigned" building
                    var locationCode = await _db.FfBuildingDirectorys
                        .Where(x => x.LocationName == "Unassigned")
                        .Select(x => x.LocationCode)
                        .SingleAsync();


                    // Create Profile
                    var initialAdminProfile = new FfUserProfile
                    {
                        FName = "Default",
                        LName = "Administrator",
                        FfUserId = initialAdmin.Id,
                        LocationCode = locationCode
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

            var defaultLocationCode = await _db.FfBuildingDirectorys
                .Where(x => x.LocationName == "Unassigned")
                .Select(x => x.LocationCode)
                .SingleAsync();

            var demoPassword = "password123";

            var demoUsers = new List<(string UserName, string Email, string Role, string FirstName, string LastName, int Unit)>
            {
                ("sky.admin", "sky.admin@fixflow.local", "Admin", "Sky", "Atlas", 100),
                ("mara.manager", "mara.manager@fixflow.local", "Manager", "Mara", "Stone", 210),
                ("eddy.employee", "eddy.employee@fixflow.local", "Employee", "Eddy", "Harbor", 305),
                ("riley.resident", "riley.resident@fixflow.local", "Resident", "Riley", "Quill", 412),
                ("penny.pending", "penny.pending@fixflow.local", "Pending", "Penny", "Drift", 509)
            };

            foreach (var demo in demoUsers)
            {
                var user = await _userManager.FindByNameAsync(demo.UserName);
                if (user == null)
                {
                    using var demoUserTransaction = await _db.Database.BeginTransactionAsync();
                    try
                    {
                        var newUser = new AppUser
                        {
                            UserName = demo.UserName,
                            Email = demo.Email,
                            EmailConfirmed = true,
                            ResetPassOnLogin = false
                        };

                        var createResult = await _userManager.CreateAsync(newUser, demoPassword);
                        if (!createResult.Succeeded)
                        {
                            throw new Exception($"Failed to create demo user '{demo.UserName}': {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                        }

                        var roleResult = await _userManager.AddToRoleAsync(newUser, demo.Role);
                        if (!roleResult.Succeeded)
                        {
                            throw new Exception($"Failed to assign role '{demo.Role}' to '{demo.UserName}': {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                        }

                        var profile = new FfUserProfile
                        {
                            FName = demo.FirstName,
                            LName = demo.LastName,
                            FfUserId = newUser.Id,
                            LocationCode = defaultLocationCode,
                            Unit = demo.Unit
                        };

                        _db.FfUserProfiles.Add(profile);
                        await _db.SaveChangesAsync();
                        await demoUserTransaction.CommitAsync();
                    }
                    catch
                    {
                        await demoUserTransaction.RollbackAsync();
                        throw;
                    }
                }
            }

            if (!await _db.FfBuildingDirectorys.AnyAsync(x => x.LocationName != "Unassigned"))
            {
                _db.FfBuildingDirectorys.AddRange(new List<FfBuildingDirectory>
                {
                    new FfBuildingDirectory
                    {
                        LocationName = "Harbor Point",
                        ComplexName = "Harbor Point",
                        BuildingNumber = 1,
                        NumUnits = 120,
                        LocationLat = 47.6062m,
                        LocationLon = -122.3321m
                    },
                    new FfBuildingDirectory
                    {
                        LocationName = "Summit Hall",
                        ComplexName = "Summit Hall",
                        BuildingNumber = 2,
                        NumUnits = 95,
                        LocationLat = 47.6205m,
                        LocationLon = -122.3493m
                    },
                    new FfBuildingDirectory
                    {
                        LocationName = "Maple Court",
                        ComplexName = "Maple Court",
                        BuildingNumber = 3,
                        NumUnits = 140,
                        LocationLat = 47.6132m,
                        LocationLon = -122.3427m
                    }
                });
                await _db.SaveChangesAsync();
            }

            if (!await _db.FfPriorityCodess.AnyAsync())
            {
                _db.FfPriorityCodess.AddRange(new List<FfPriorityCodes>
                {
                    new FfPriorityCodes { Code = 1, PriorityName = "Low" },
                    new FfPriorityCodes { Code = 2, PriorityName = "Medium" },
                    new FfPriorityCodes { Code = 3, PriorityName = "High" }
                });
                await _db.SaveChangesAsync();
            }

            if (!await _db.FfStatusCodes.AnyAsync())
            {
                _db.FfStatusCodes.AddRange(new List<FfStatusCodes>
                {
                    new FfStatusCodes { Code = 1, StatusName = "Submitted" },
                    new FfStatusCodes { Code = 2, StatusName = "In Review" },
                    new FfStatusCodes { Code = 3, StatusName = "Assigned" },
                    new FfStatusCodes { Code = 4, StatusName = "In Progress" },
                    new FfStatusCodes { Code = 5, StatusName = "On Hold" },
                    new FfStatusCodes { Code = 6, StatusName = "Completed" }
                });
                await _db.SaveChangesAsync();
            }

            if (!await _db.FfTicketTypess.AnyAsync())
            {
                _db.FfTicketTypess.AddRange(new List<FfTicketTypes>
                {
                    new FfTicketTypes { TypeName = "Plumbing" },
                    new FfTicketTypes { TypeName = "Electrical" },
                    new FfTicketTypes { TypeName = "HVAC" },
                    new FfTicketTypes { TypeName = "Maintenance" },
                    new FfTicketTypes { TypeName = "Safety" },
                    new FfTicketTypes { TypeName = "Cleaning" }
                });
                await _db.SaveChangesAsync();
            }

            if (!await _db.FfTicketRegisters.AnyAsync())
            {
                var adminUser = await _userManager.FindByNameAsync("sky.admin");
                var managerUser = await _userManager.FindByNameAsync("mara.manager");
                var employeeUser = await _userManager.FindByNameAsync("eddy.employee");
                var residentUser = await _userManager.FindByNameAsync("riley.resident");
                var pendingUser = await _userManager.FindByNameAsync("penny.pending");

                var locations = await _db.FfBuildingDirectorys
                    .Where(x => x.LocationName != "Unassigned")
                    .ToListAsync();

                var ticketTypes = await _db.FfTicketTypess.ToListAsync();
                var statusCodes = await _db.FfStatusCodes.ToListAsync();
                var priorityCodes = await _db.FfPriorityCodess.ToListAsync();

                var submittedStatus = statusCodes.First(x => x.StatusName == "Submitted");
                var reviewStatus = statusCodes.First(x => x.StatusName == "In Review");
                var assignedStatus = statusCodes.First(x => x.StatusName == "Assigned");
                var progressStatus = statusCodes.First(x => x.StatusName == "In Progress");
                var completedStatus = statusCodes.First(x => x.StatusName == "Completed");

                var highPriority = priorityCodes.First(x => x.PriorityName == "High");
                var mediumPriority = priorityCodes.First(x => x.PriorityName == "Medium");
                var lowPriority = priorityCodes.First(x => x.PriorityName == "Low");

                var plumbing = ticketTypes.First(x => x.TypeName == "Plumbing");
                var electrical = ticketTypes.First(x => x.TypeName == "Electrical");
                var hvac = ticketTypes.First(x => x.TypeName == "HVAC");
                var maintenance = ticketTypes.First(x => x.TypeName == "Maintenance");
                var safety = ticketTypes.First(x => x.TypeName == "Safety");
                var cleaning = ticketTypes.First(x => x.TypeName == "Cleaning");

                var now = DateTime.UtcNow;

                var tickets = new List<FfTicketRegister>
                {
                    new FfTicketRegister
                    {
                        TicketId = Guid.NewGuid(),
                        TicketShortCode = "T-1001",
                        EnteredBy = employeeUser?.Id ?? string.Empty,
                        RequestedBy = residentUser?.Id ?? string.Empty,
                        Location = locations[0].LocationCode,
                        Unit = 120,
                        TicketTroubleType = plumbing.Code,
                        TicketStatus = assignedStatus.Code,
                        TicketPriority = highPriority.Code
                    },
                    new FfTicketRegister
                    {
                        TicketId = Guid.NewGuid(),
                        TicketShortCode = "T-1002",
                        EnteredBy = employeeUser?.Id ?? string.Empty,
                        RequestedBy = residentUser?.Id ?? string.Empty,
                        Location = locations[1].LocationCode,
                        Unit = 305,
                        TicketTroubleType = electrical.Code,
                        TicketStatus = progressStatus.Code,
                        TicketPriority = mediumPriority.Code
                    },
                    new FfTicketRegister
                    {
                        TicketId = Guid.NewGuid(),
                        TicketShortCode = "T-1003",
                        EnteredBy = managerUser?.Id ?? string.Empty,
                        RequestedBy = residentUser?.Id ?? string.Empty,
                        Location = locations[2].LocationCode,
                        Unit = 412,
                        TicketTroubleType = hvac.Code,
                        TicketStatus = reviewStatus.Code,
                        TicketPriority = highPriority.Code
                    },
                    new FfTicketRegister
                    {
                        TicketId = Guid.NewGuid(),
                        TicketShortCode = "T-1004",
                        EnteredBy = adminUser?.Id ?? string.Empty,
                        RequestedBy = residentUser?.Id ?? string.Empty,
                        Location = locations[0].LocationCode,
                        Unit = 511,
                        TicketTroubleType = maintenance.Code,
                        TicketStatus = completedStatus.Code,
                        TicketPriority = lowPriority.Code
                    },
                    new FfTicketRegister
                    {
                        TicketId = Guid.NewGuid(),
                        TicketShortCode = "T-1005",
                        EnteredBy = employeeUser?.Id ?? string.Empty,
                        RequestedBy = pendingUser?.Id ?? string.Empty,
                        Location = locations[1].LocationCode,
                        Unit = 210,
                        TicketTroubleType = safety.Code,
                        TicketStatus = submittedStatus.Code,
                        TicketPriority = highPriority.Code
                    },
                    new FfTicketRegister
                    {
                        TicketId = Guid.NewGuid(),
                        TicketShortCode = "T-1006",
                        EnteredBy = employeeUser?.Id ?? string.Empty,
                        RequestedBy = residentUser?.Id ?? string.Empty,
                        Location = locations[2].LocationCode,
                        Unit = 305,
                        TicketTroubleType = cleaning.Code,
                        TicketStatus = assignedStatus.Code,
                        TicketPriority = lowPriority.Code
                    }
                };

                _db.FfTicketRegisters.AddRange(tickets);
                await _db.SaveChangesAsync();

                var flows = new List<FfTicketFlow>
                {
                    new FfTicketFlow
                    {
                        TicketId = tickets[0].TicketId,
                        NewTicketStatus = assignedStatus.Code,
                        NewAssignee = employeeUser?.Id ?? string.Empty,
                        TimeStamp = now.AddHours(-6)
                    },
                    new FfTicketFlow
                    {
                        TicketId = tickets[1].TicketId,
                        NewTicketStatus = progressStatus.Code,
                        NewAssignee = employeeUser?.Id ?? string.Empty,
                        TimeStamp = now.AddHours(-3)
                    },
                    new FfTicketFlow
                    {
                        TicketId = tickets[2].TicketId,
                        NewTicketStatus = reviewStatus.Code,
                        NewAssignee = managerUser?.Id ?? string.Empty,
                        TimeStamp = now.AddHours(-10)
                    },
                    new FfTicketFlow
                    {
                        TicketId = tickets[3].TicketId,
                        NewTicketStatus = completedStatus.Code,
                        NewAssignee = employeeUser?.Id ?? string.Empty,
                        TimeStamp = now.AddDays(-1)
                    },
                    new FfTicketFlow
                    {
                        TicketId = tickets[4].TicketId,
                        NewTicketStatus = submittedStatus.Code,
                        NewAssignee = pendingUser?.Id ?? string.Empty,
                        TimeStamp = now.AddHours(-1)
                    },
                    new FfTicketFlow
                    {
                        TicketId = tickets[5].TicketId,
                        NewTicketStatus = assignedStatus.Code,
                        NewAssignee = employeeUser?.Id ?? string.Empty,
                        TimeStamp = now.AddHours(-4)
                    }
                };

                _db.FfTicketFlows.AddRange(flows);
                await _db.SaveChangesAsync();

                _db.FfExternalNotess.AddRange(new List<FfExternalNotes>
                {
                    new FfExternalNotes
                    {
                        TicketId = tickets[0].TicketId,
                        CreatedBy = "riley.resident",
                        TimeStamp = now.AddHours(-5),
                        Content = "Water pooling under the sink. It is getting worse."
                    },
                    new FfExternalNotes
                    {
                        TicketId = tickets[1].TicketId,
                        CreatedBy = "riley.resident",
                        TimeStamp = now.AddHours(-2),
                        Content = "Light flickers when the microwave runs."
                    },
                    new FfExternalNotes
                    {
                        TicketId = tickets[2].TicketId,
                        CreatedBy = "riley.resident",
                        TimeStamp = now.AddHours(-9),
                        Content = "Airflow is weak in the living room vents."
                    }
                });

                _db.FfInternalNotess.AddRange(new List<FfInternalNotes>
                {
                    new FfInternalNotes
                    {
                        TicketId = tickets[0].TicketId,
                        CreatedBy = "eddy.employee",
                        TimeStamp = now.AddHours(-4),
                        Content = "Assigned plumber. Parts might be needed."
                    },
                    new FfInternalNotes
                    {
                        TicketId = tickets[3].TicketId,
                        CreatedBy = "mara.manager",
                        TimeStamp = now.AddDays(-1).AddHours(2),
                        Content = "Closed after resident confirmed resolution."
                    }
                });

                await _db.SaveChangesAsync();
            }
        }
    }
}