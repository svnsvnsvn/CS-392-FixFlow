using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using fixflow.web.Data;
using fixflow.web.Services;

namespace fixflow.web.Pages.Tickets
{
    public class EditModel : PageModel
    {
        private readonly ITicketService _ticketService;

        public EditModel(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [BindProperty]
        public FfTicketRegister Ticket { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var ticketResult = await _ticketService.GetTicketById(id);
            if (!ticketResult.Success || ticketResult.Data == null)
            {
                return NotFound();
            }

            Ticket = ticketResult.Data;
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
