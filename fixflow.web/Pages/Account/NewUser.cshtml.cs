using fixflow.web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using fixflow.web.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using fixflow.web.Services;

namespace fixflow.web.Pages.Account;

public class NewUserModel : PageModel
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IAdminService _adminService;

    public NewUserModel(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IAdminService adminService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _adminService = adminService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required, Display(Name = "First name")]
        public string FirstName { get; set; } = string.Empty;
        [Required, Display(Name = "Last name")]
        public string LastName { get; set; } = string.Empty;
        [Phone, Display(Name = "Phone (optional)")]
        public string? PhoneNumber { get; set; }
        [Required, EmailAddress, Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        bool availableUserNameFound = false;
        int increment = 0;
        string newUserName = Input.FirstName + "." + Input.LastName;
        do
        {
            var validRequestor = await _userManager.FindByNameAsync(newUserName);
            if (validRequestor != null)
            {
                increment++;
                newUserName = Input.FirstName + "." + Input.LastName + increment.ToString();
            }
            else
            {
                availableUserNameFound = true;
            }
        } while (!availableUserNameFound);

        // Build new user from form inputs
        var user = new AppUser
        {
            UserName = newUserName,
            Email = Input.Email,
            PhoneNumber = Input.PhoneNumber,
            ResetPassOnLogin = true,
            LockoutEnabled = true
        };

        // Create user
        var resultU = await _userManager.CreateAsync(user);
        if (!resultU.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "User creation failed.");
            return Page();
        }

        // New user assigned pending role while awaiting full onboard.
        var resultUR = await _userManager.AddToRoleAsync(user, RoleTypes.Pending.ToString());
        if (!resultUR.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            ModelState.AddModelError(string.Empty, "User role assignment failed.");
            return Page();
        }

        var defaultBuildingCodeResult = await _adminService.GetUnassignedBuildingCode();
        if (!defaultBuildingCodeResult.Success)
        {
            await _userManager.DeleteAsync(user);
            ModelState.AddModelError(string.Empty, defaultBuildingCodeResult.Error ?? "Unassigned building was not found.");
            return Page();
        }

        var addProfileResult = await _adminService.AddUserProfile(new FfUserProfile
        {
            FName = Input.FirstName,
            LName = Input.LastName,
            FfUserId = user.Id,
            LocationCode = defaultBuildingCodeResult.Data
        });
        if (!addProfileResult.Success)
        {
            await _userManager.DeleteAsync(user);
            ModelState.AddModelError(string.Empty, addProfileResult.Error ?? "User profile creation failed.");
            return Page();
        }


        // New user must set password; send them to Profile with token.
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        return RedirectToPage("/Account/Profile", new { userId = user.Id, token });
    }
}
