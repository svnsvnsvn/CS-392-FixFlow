using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using fixflow.web.Domain.Enums;

namespace fixflow.web.Pages.Admin
{
    /// <summary>Legacy route; user management lives on <see cref="UsersModel"/>.</summary>
    [Authorize(Roles = nameof(RoleTypes.Admin))]
    public class AdminUserToolsModel : PageModel
    {
        public IActionResult OnGet() => RedirectToPage("/Admin/Users");
    }
}
