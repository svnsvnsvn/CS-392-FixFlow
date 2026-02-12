using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using fixflow.web.Data;

namespace fixflow.web.Pages.Tickets
{
    public class ViewModel : PageModel
    {
        private readonly FfDbContext _context;

        public ViewModel(FfDbContext context)
        {
            _context = context;
        }

        public FfTicketRegister Ticket { get; set; } = default!;
        public Guid TicketId { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            TicketId = id;
            var ticket = await _context.FfTicketRegister.FirstOrDefaultAsync(m => m.TicketId == id);

            if (ticket == null)
            {
                return NotFound();
            }

            Ticket = ticket;
            return Page();
        }
    }
}
