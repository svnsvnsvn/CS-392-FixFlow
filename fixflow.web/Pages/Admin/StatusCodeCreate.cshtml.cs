using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using fixflow.web.Domain.Constants;
using fixflow.web.Domain.Enums;
namespace fixflow.web.Pages.Admin
{
    [Authorize(Roles = nameof(RoleTypes.Admin))]
    public class StatusCodeCreateModel : PageModel
    {
        [BindProperty]
        public StatusCodeInput Input { get; set; } = new();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // TODO: Wire to AdminService to create the status code.
            return RedirectToPage("./StatusCodes");
        }
    }

    public class StatusCodeInput
    {
        public int StatusCode { get; set; }
        public string StatusName { get; set; } = string.Empty;
    }
}
