using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using fixflow.web.Data;


namespace fixflow.web.Pages.Admin

{
        [Authorize(Roles = "Admin")] // Restrict access to only admin users
        public class AdminUserToolModel : PageModel
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;

            public AdminUserToolModel(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
            {
                _userManager = userManager;
                _roleManager = roleManager;
            }

            public async Task<string> GetUserRoleAsync(AppUser user)
            {
                var roles = await _userManager.GetRolesAsync(user);
                return roles.FirstOrDefault() ?? "No Role";
            }

            public async Task<IActionResult> OnPostUpdateDataAsync()
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                var data = JsonSerializer.Deserialize<DataMule>(body);
                bool changedPassword = false;
                bool changedLockout = false;
                bool changedRole = false;

                //Write to DB here using data.
                var userToModify = await _userManager.FindByIdAsync(data.UserID);     // Find user in DB
                if (userToModify == null)       // If user does not exist, return error result
                {
                    // Return an error packet and RESTful status code.  No idea what that is by ChatGPT says I should
                    return new JsonResult(new { success = false, reason = "User " + data.UserID + " not found." })
                    {
                        StatusCode = 404
                    };
                }


                // Process password reset if directed
                if (data.ResetPswd == true)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(userToModify);       // Generate token to authorize password reset
                    var result = await _userManager.ResetPasswordAsync(userToModify, token, "P@ssw0rd!");   // Set to default RESET password
                    if (result.Succeeded == true)      // Return an error packet
                    {
                        changedPassword = true;
                    }
                    else
                    {
                        return new JsonResult(new { success = false, reason = "User " + data.UserID + " password could not be reset." });
                    }
                }


            // Set account lockout and write to DB as required
            if (data.LockAccount != userToModify.LockoutEnabled)
            {
                if (data.LockAccount == true)
                {
                    userToModify.LockoutEnabled = true;     // Set lockout flag true
                    userToModify.LockoutEnd = DateTimeOffset.MaxValue;      // Set lockout to infinity and/or beyond
                }
                else
                {
                    userToModify.LockoutEnabled = false;     // Set lockout flag true
                    userToModify.LockoutEnd = DateTimeOffset.UtcNow;      // Set lockout to right meow!
                }
                changedLockout = true;
            }
            if (changedLockout)     // Write to DB
            {
                var result = await _userManager.UpdateAsync(userToModify);
                if (result.Succeeded == false)
                {
                    changedLockout = false;
                }
            }


            // Check user roles for change or multiple roles.  If multiple roles remove all except specified.
            // Will only update DB if role new role is different than current.
            var userToModifyRole = await _userManager.GetRolesAsync(userToModify);      // Get current roles
            bool changeRole = true;        // Flag to signal that current role is not same as new role, thus changes need to be made to roles


            if (userToModifyRole.Count() > 1)  // Defense against multiple roles. Should only have 1 or 0 (Non-User
            {
                changeRole = true;      // Yeah this is dumb, I dont like it either, thinking about better order to do this
            }
            else  // 1 or 0 roles currently assigned
            {
                if ((userToModifyRole == null) && (data.NewRole == "Pending"))
                {
                    changeRole = false;
                }
                
                if (userToModifyRole.FirstOrDefault() == data.NewRole)
                {
                    changeRole = false;
                }
            }
            if (changeRole)     // If role was not identical to current, or there were more than one roles, then remove all roles and add new role only
            {
                // Remove all roles found
                await _userManager.RemoveFromRolesAsync(userToModify, userToModifyRole);
                // Add new role received
                if (data.NewRole != "Pending")
                {
                    await _userManager.AddToRoleAsync(userToModify, data.NewRole);
                    changedRole = true;
                }
                else
                {
                    changedRole = false;
                }
            }

            return new JsonResult(new { success = true, passwordChanged = changedPassword, lockoutChanged = changedLockout, roleChanged = changedRole });
            }

            public class DataMule
        {
                public string UserID { get; set; } = string.Empty;
                public bool ResetPswd { get; set; }
                public bool LockAccount { get; set; }
            public string NewRole { get; set; } = string.Empty;
        }

        public List<AppUser> Users { get; set; }

            public async Task<IActionResult> OnGetAsync()
            {
                Users = _userManager.Users.ToList();
                return Page();
            }
        }
    }