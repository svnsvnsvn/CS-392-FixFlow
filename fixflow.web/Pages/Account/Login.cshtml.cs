using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using fixflow.web.Data;

namespace fixflow.web.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;

    public LoginModel(SignInManager<AppUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [BindProperty]
    public LoginInput Input { get; set; } = new();

    public bool LoginFailed { get; set; }

    public class LoginInput
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, false, false);
        if (result.Succeeded)
        {
            return RedirectToPage("/Index"); // redirect on successful login
        }
        LoginFailed = true;
        return Page();
    }
}
