using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace fixflow.web.Pages.Admin
{
    public class TicketTypeCreateModel : PageModel
    {
        [BindProperty]
        public TicketTypeInput Input { get; set; } = new();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // TODO: Wire to AdminService to create the ticket type.
            return RedirectToPage("./TicketTypes");
        }
    }

    public class TicketTypeInput
    {
        public string TypeName { get; set; } = string.Empty;
    }
}
