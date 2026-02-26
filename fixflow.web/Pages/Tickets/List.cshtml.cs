using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using fixflow.web.Data;
using fixflow.web.Domain.Constants;
using fixflow.web.Domain.Enums;
using fixflow.web.Services;

namespace fixflow.web.Pages.Tickets
{
    public class ListModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITicketService _ticketService;
        private readonly FfDbContext _context;

        public ListModel(UserManager<AppUser> userManager, ITicketService ticketService, FfDbContext context)
        {
            _userManager = userManager;
            _ticketService = ticketService;
            _context = context;
        }

        public IList<FfTicketRegister> Tickets { get; set; } = default!;
        public Dictionary<Guid, string> TicketAssignees { get; set} = new();

        public async Task OnGetAsync()
        {
            // Load tickets directly from DB
            Tickets = await _context.FfTicketRegisters
                .Include(t => t.TicketType)
                .Include(t => t.PriorityCode)
                .Include(t => t.StatusCode)
                .Include(t => t.Building)
                .Include(t => t.RequestedByUser)
                .AsNoTracking()
                .ToListAsync();

            // Get latest assignees for each ticket
            var ticketIds = Tickets.Select(t => t.TicketId).ToList();
            var latestFlows = await _context.FfTicketFlows
                .Where(f => ticketIds.Contains(f.TicketId))
                .GroupBy(f => f.TicketId)
                .Select(g => g.OrderByDescending(f => f.TimeStamp).FirstOrDefault())
                .ToListAsync();

            var userIds = latestFlows.Where(f => f != null && f.NewAssignee != null)
                .Select(f => f!.NewAssignee)
                .Distinct()
                .ToList();

            var userProfiles = await _context.FfUserProfiles
                .Where(p => userIds.Contains(p.FfUserId))
                .ToDictionaryAsync(p => p.FfUserId, p => $"{p.FName} {p.LName}".Trim());

            TicketAssignees = latestFlows
                .Where(f => f != null && f.NewAssignee != null)
                .ToDictionary(
                    f => f!.TicketId,
                    f => userProfiles.TryGetValue(f.NewAssignee!, out var name) ? name : "Unknown"
                );
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

            // Determine user role
            RoleTypes userRole = RoleTypes.Employee;

            // Get "Assigned" status code
            var assignedStatus = await _context.FfStatusCodes
                .FirstOrDefaultAsync(s => s.StatusName == TicketStatusNames.Assigned);

            if (assignedStatus == null)
            {
                TempData["ErrorMessage"] = "System configuration error: Assigned status not found.";
                return RedirectToPage();
            }

            var result = await _ticketService.ReassignTicket(
                user.Id,
                userRole,
                ticketId,
                user.Id, // Assign to self
                assignedStatus.Code
            );

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