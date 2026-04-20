using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using fixflow.web.Data;
using fixflow.web.Domain.Enums;
using fixflow.web.Services;

namespace fixflow.web.Pages.Tickets
{
    public class ListModel : AppPageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITicketService _ticketService;

        public ListModel(UserManager<AppUser> userManager, ITicketService ticketService)
        {
            _userManager = userManager;
            _ticketService = ticketService;
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

            var bundleResult = await _ticketService.GetTicketListBundle();
            if (!bundleResult.Success || bundleResult.Data == null)
            {
                Tickets = new List<FfTicketRegister>();
                TicketAssignees = new Dictionary<Guid, string>();
                return;
            }

            Tickets = bundleResult.Data.Tickets;

            // If resident then only show tickets requested by them
            if (LoggedInUser.Role == RoleTypes.Resident)
            {
                Tickets = Tickets
                .Where(x => x.RequestedBy == LoggedInUser.UserId)
                .ToList();
            }

            var allFlows = bundleResult.Data.Flows;

            var latestFlows = allFlows
                .GroupBy(f => f.TicketId)
                .Select(g => g.OrderByDescending(f => f.TimeStamp).First())
                .ToList();

            var userProfiles = bundleResult.Data.ProfileDisplayNames;

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

            var result = await _ticketService.ReassignTicket(
                user.Id,
                userRole,
                ticketId,
                user.Id, // Assign to self
                assignedCode
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