using fixflow.web.Data;
using fixflow.web.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace fixflow.web.Pages.Account;

public class NewUserModel : PageModel
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly FfDbContext _db;

    public NewUserModel(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, FfDbContext db)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Phone]
        public string? PhoneNumber { get; set; }
        [Required, EmailAddress]
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

        // Create transaction object to user creation
        using var userCreationTransaction = await _db.Database.BeginTransactionAsync();
        try
        {
            // Create user
            var resultU = await _userManager.CreateAsync(user);
            if (!resultU.Succeeded)
            {
                throw new Exception("User creation failed.");
            }

            // New user assigned pending role while awaiting full onboard.        
            var resultUR = await _userManager.AddToRoleAsync(user, RoleNames.Pending);
            if (!resultUR.Succeeded)
            {
                throw new Exception("User role assignment failed.");
            }

            // Get the "Unassigned" building code for profile
            var defaultBuilding = await _db.FfBuildingDirectorys.SingleAsync(b => b.LocationName == "Unassigned");
            if (defaultBuilding == null)
            {
                throw new Exception("User creation failed. Unassigned building not found.");
            }
            
            // Build user profile
            var userProfile = new FfUserProfile
            {
                FName = Input.FirstName,
                LName = Input.LastName,
                FfUserId = user.Id,
                LocationCode = defaultBuilding.LocationCode
            };


            _db.FfUserProfiles.Add(userProfile);
            await _db.SaveChangesAsync();

            await userCreationTransaction.CommitAsync();
        }
        catch
        {
            await userCreationTransaction.RollbackAsync();          // Stop db writes if something failed. Prevent half transactions.
            throw;
        }


        // New user is a user, generate token and forward to user profile page to setup password.
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        return RedirectToPage("/Account/Profile", new { userId = user.Id, token });
    }
}
