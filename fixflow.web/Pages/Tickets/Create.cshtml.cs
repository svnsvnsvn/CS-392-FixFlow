using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using fixflow.web.Data;

namespace fixflow.web.Pages.Tickets
{
    public class CreateModel : PageModel
    {
        private readonly FfDbContext _context;

        public CreateModel(FfDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public FfTicketRegister Ticket { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Add ticket creation logic here

            return RedirectToPage("./List");
        }
    }
}
