using fixflow.web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace fixflow.Web.Pages.Account;

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

        string newUserName = Input.FirstName + "." + Input.LastName;

        // ***************** Code to prevent duplicate users, i.e. john.smith  next should be john.smith1


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
            var resultUR = await _userManager.AddToRoleAsync(user, "Pending");
            if (!resultUR.Succeeded)
            {
                throw new Exception("User role assignment failed.");
            }

            // Build user profile
            var userProfile = new FfUserProfile
            {
                FName = Input.FirstName,
                LName = Input.LastName,
                FfUserId = user.Id,
                Location = null
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
