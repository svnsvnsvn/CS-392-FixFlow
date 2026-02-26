using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using fixflow.web.Data;
using fixflow.web.Domain.Constants;
using fixflow.web.Services;

namespace fixflow.web.Pages.Tickets
{
    public class ListModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITicketService _ticketService;

        public ListModel(UserManager<AppUser> userManager, ITicketService ticketService)
        {
            _userManager = userManager;
            _ticketService = ticketService;
        }

        public IList<FfTicketRegister> Tickets { get; set; } = default!;
        public Dictionary<Guid, string> TicketAssignees { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Load tickets list here
            Tickets = await _ticketService.GetTicketsForListAsync();

            var ticketIds = Tickets.Select(ticket => ticket.TicketId).ToList();
            TicketAssignees = await _ticketService.GetLatestAssigneesAsync(ticketIds);
        }

        public async Task<IActionResult> OnPostPickUpTicketAsync(Guid ticketId)
        {
            // Verify user is a technician
            if (!User.IsInRole(RoleNames.Employee))
            {
                return Forbid();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var result = await _ticketService.AssignTicketAsync(ticketId, user.Id);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Error ?? "Ticket assignment failed.";
                return RedirectToPage();
            }

            TempData["SuccessMessage"] = "Ticket assigned to you successfully!";
            return RedirectToPage();
        }
    }
}
