using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using fixflow.web.Data;
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
        public Dictionary<Guid, string> TicketAssignees { get; set;  } = new();

        public bool ShowOperationsStats { get; set; }
        public int TotalTickets { get; set; }
        public int PendingTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int CompletedToday { get; set; }

        public bool ShowTechnicianWorkspaceIntro { get; set; }

        public async Task OnGetAsync()
        {
            // Resident/Pending redirect off List; staff GET /Dashboard -> List: Program.cs middleware.

            // TODO (Adam): Load tickets (and assignee flows/profiles) via a service instead of FfDbContext.
            // ITicketService has no list-tickets API yet; add one or extend an existing service.
            Tickets = await _context.FfTicketRegisters
                .Include(t => t.TicketType)
                .Include(t => t.PriorityCode)
                .Include(t => t.StatusCode)
                .Include(t => t.Building)
                .Include(t => t.RequestedByUser)
                .AsNoTracking()
                .ToListAsync();

            var ticketIds = Tickets.Select(t => t.TicketId).ToList();
            var allFlows = ticketIds.Count == 0
                ? new List<FfTicketFlow>()
                : await _context.FfTicketFlows
                    .Where(f => ticketIds.Contains(f.TicketId))
                    .AsNoTracking()
                    .ToListAsync();

            var latestFlows = allFlows
                .GroupBy(f => f.TicketId)
                .Select(g => g.OrderByDescending(f => f.TimeStamp).First())
                .ToList();

            var userIds = latestFlows
                .Where(f => !string.IsNullOrEmpty(f.NewAssignee))
                .Select(f => f.NewAssignee)
                .Distinct()
                .ToList();

            var userProfiles = userIds.Count == 0
                ? new Dictionary<string, string>()
                : await _context.FfUserProfiles
                    .Where(p => userIds.Contains(p.FfUserId))
                    .ToDictionaryAsync(p => p.FfUserId, p => $"{p.FName} {p.LName}".Trim());

            TicketAssignees = latestFlows
                .Where(f => !string.IsNullOrEmpty(f.NewAssignee))
                .ToDictionary(
                    f => f.TicketId,
                    f => userProfiles.TryGetValue(f.NewAssignee, out var name) ? name : "Unknown");

            if (User.IsInRole(RoleTypes.Admin.ToString()) || User.IsInRole(RoleTypes.Manager.ToString()))
            {
                ShowOperationsStats = true;
                var pendingCode = (await _ticketService.GetStatusCode("Submitted")).Data;
                var completedCode = (await _ticketService.GetStatusCode("Completed")).Data;
                var inProgressCode = (await _ticketService.GetStatusCode("In Progress")).Data;

                TotalTickets = Tickets.Count;
                PendingTickets = Tickets.Count(t => t.TicketStatus == pendingCode);
                InProgressTickets = Tickets.Count(t => t.TicketStatus == inProgressCode);
                CompletedToday = Tickets.Count(ticket => ticket.TicketStatus == completedCode &&
                    allFlows
                        .Where(flow => flow.TicketId == ticket.TicketId)
                        .OrderByDescending(flow => flow.TimeStamp)
                        .Select(flow => flow.TimeStamp)
                        .FirstOrDefault()
                        .Date == DateTime.UtcNow.Date);
            }
            else if (User.IsInRole(RoleTypes.Employee.ToString()))
            {
                ShowTechnicianWorkspaceIntro = true;
            }
        }

        public async Task<IActionResult> OnPostPickUpTicketAsync(Guid ticketId)
        {
            // Verify user is a technician
            if (!User.IsInRole(RoleTypes.Employee.ToString()))
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
            var assignedCode = _ticketService.GetStatusCode("Assigned").Result.Data;

            var assignedStatus = await _context.FfStatusCodes
                .FirstOrDefaultAsync(s => s.StatusCode == assignedCode);

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
                assignedStatus.Id
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