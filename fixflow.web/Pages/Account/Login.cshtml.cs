using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using fixflow.web.Data;

namespace fixflow.Web.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(
        SignInManager<AppUser> signInManager,
        ILogger<LoginModel> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    public class LoginInput
    {
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public bool LoginFailed { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(
        string userName,
        string password)
    {
        _logger.LogCritical(
            "POST user={User} passLen={Len}",
            userName,
            password?.Length ?? -1
        );

        var result = await _signInManager.PasswordSignInAsync(
            userName,
            password,
            isPersistent: false,
            lockoutOnFailure: false
        );

        if (result.Succeeded)
            return RedirectToPage("/Account/Home");

        LoginFailed = true;
        return Page();
    }
}
