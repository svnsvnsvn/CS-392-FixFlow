using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace fixflow.web.Pages.Admin
{
    public class PriorityCodeCreateModel : PageModel
    {
        [BindProperty]
        public PriorityCodeInput Input { get; set; } = new();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // TODO: Wire to AdminService to create the priority code.
            return RedirectToPage("./PriorityCodes");
        }
    }

    public class PriorityCodeInput
    {
        public int PriorityCode { get; set; }
        public string PriorityName { get; set; } = string.Empty;
    }
}
