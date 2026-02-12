using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using fixflow.web.Data;

namespace fixflow.web.Pages.Tickets
{
    public class EditModel : PageModel
    {
        private readonly FfDbContext _context;

        public EditModel(FfDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public FfTicketRegister Ticket { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var ticket = await _context.FfTicketRegister.FirstOrDefaultAsync(m => m.TicketId == id);

            if (ticket == null)
            {
                return NotFound();
            }

            Ticket = ticket;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Add ticket update logic here

            return RedirectToPage("./View", new { id = Ticket.TicketId });
        }
    }
}
