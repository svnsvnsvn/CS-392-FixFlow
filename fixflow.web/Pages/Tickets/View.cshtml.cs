using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using fixflow.web.Data;
using fixflow.web.Services;

namespace fixflow.web.Pages.Tickets
{
    public class ViewModel : PageModel
    {
        private readonly ITicketService _ticketService;

        public ViewModel(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public FfTicketRegister Ticket { get; set; } = default!;
        public Guid TicketId { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            TicketId = id;
            var ticketResult = await _ticketService.GetTicketById(id);
            if (!ticketResult.Success || ticketResult.Data == null)
            {
                return NotFound();
            }

            Ticket = ticketResult.Data;
            return Page();
        }
    }
}
