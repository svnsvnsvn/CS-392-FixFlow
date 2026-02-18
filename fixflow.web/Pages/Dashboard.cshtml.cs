using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using fixflow.web.Data;
using fixflow.web.Domain.Constants;

namespace fixflow.web.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly FfDbContext _context;

        public DashboardModel(FfDbContext context)
        {
            _context = context;
        }

        // User Properties
        public string UserName { get; set; } = "Demo User";
        public string UserRole { get; set; } = "Client"; // Client, Technician, Manager, Admin
        public string UserInitials { get; set; } = "DU";
        public string WelcomeMessage { get; set; } = string.Empty;
        
        // Stats (for Manager/Admin)
        public int TotalTickets { get; set; }
        public int PendingTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int CompletedToday { get; set; }

        // Section titles that change based on role
        public string TicketsSectionTitle { get; set; } = "My Tickets";
        public string TicketsViewAllLabel { get; set; } = "My Tickets";
        public string TicketsViewAllUrl { get; set; } = "/Tickets/List";

        // Tickets list
        public List<TicketViewModel> Tickets { get; set; } = new();
        public List<TicketViewModel> RecentTickets { get; set; } = new();

        // Dashboard modules
        public List<DashboardAppointment> UpcomingAppointments { get; set; } = new();
        public List<DashboardActivityItem> RecentActivity { get; set; } = new();
        public List<DashboardAnnouncement> Announcements { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string? role = null)
        {
            // Get user info (from authentication)
            UserName = User.Identity?.Name ?? "Demo User";
            var userRole = role ?? ResolveRole();
            UserRole = userRole;
            UserInitials = GetInitials(UserName);

            SetRoleCopy(userRole);
            await BuildDashboardFromDbAsync(userRole);

            return Page();
        }

        private string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "??";
            
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[1][0]}".ToUpper();
            
            return name.Length >= 2 ? name.Substring(0, 2).ToUpper() : name.ToUpper();
        }

        private void SetRoleCopy(string role)
        {
            switch (role)
            {
                case "Technician":
                    WelcomeMessage = "You have assigned tasks waiting in your queue.";
                    TicketsSectionTitle = "My Assignments";
                    TicketsViewAllLabel = "My Assignments";
                    TicketsViewAllUrl = "/Tickets/List";
                    break;
                case "Manager":
                    WelcomeMessage = "Manage tickets, assign technicians, and oversee operations.";
                    TicketsSectionTitle = "All Tickets";
                    TicketsViewAllLabel = "All Tickets";
                    TicketsViewAllUrl = "/Tickets/List";
                    break;
                case "Admin":
                    WelcomeMessage = "System overview and administrative controls.";
                    TicketsSectionTitle = "Recent Tickets";
                    TicketsViewAllLabel = "All Tickets";
                    TicketsViewAllUrl = "/Tickets/List";
                    break;
                default:
                    WelcomeMessage = "Track your maintenance requests and submit new tickets.";
                    TicketsSectionTitle = "My Tickets";
                    TicketsViewAllLabel = "My Tickets";
                    TicketsViewAllUrl = "/Tickets/List";
                    break;
            }
        }

        private string ResolveRole()
        {
            if (User.IsInRole(RoleNames.Admin))
            {
                return "Admin";
            }

            if (User.IsInRole(RoleNames.Manager))
            {
                return "Manager";
            }

            if (User.IsInRole(RoleNames.Employee))
            {
                return "Technician";
            }

            if (User.IsInRole(RoleNames.Resident) || User.IsInRole(RoleNames.Pending))
            {
                return "Client";
            }

            return "Client";
        }

        private async Task BuildDashboardFromDbAsync(string role)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            var ticketsQuery = _context.FfTicketRegisters
                .Include(ticket => ticket.TicketType)
                .Include(ticket => ticket.PriorityCode)
                .Include(ticket => ticket.StatusCode)
                .Include(ticket => ticket.Building)
                .Include(ticket => ticket.RequestedByUser)
                .AsNoTracking();

            if (role == "Client")
            {
                ticketsQuery = ticketsQuery.Where(ticket => ticket.RequestedBy == userId);
            }
            else if (role == "Technician")
            {
                ticketsQuery = ticketsQuery.Where(ticket => ticket.EnteredBy == userId);
            }

            var ticketsData = await ticketsQuery.ToListAsync();

            var ticketIds = ticketsData.Select(ticket => ticket.TicketId).ToList();
            var flows = await _context.FfTicketFlows
                .Where(flow => ticketIds.Contains(flow.TicketId))
                .AsNoTracking()
                .ToListAsync();

            var statusCodes = await _context.FfStatusCodes.AsNoTracking().ToListAsync();
            var profileLookup = await _context.FfUserProfiles.AsNoTracking()
                .ToDictionaryAsync(profile => profile.FfUserId, profile => profile);

            Tickets = ticketsData.Select(ticket =>
            {
                var created = flows.Where(flow => flow.TicketId == ticket.TicketId)
                    .OrderBy(flow => flow.TimeStamp)
                    .Select(flow => flow.TimeStamp)
                    .FirstOrDefault();

                var requestedByName = profileLookup.TryGetValue(ticket.RequestedBy, out var profile)
                    ? $"{profile.FName} {profile.LName}".Trim()
                    : ticket.RequestedBy;

                var ticketTypeName = ticket.TicketType?.TypeName ?? "Maintenance";
                var buildingName = ticket.Building?.LocationName ?? "Unknown building";

                return new TicketViewModel
                {
                    Id = string.IsNullOrWhiteSpace(ticket.TicketShortCode)
                        ? ticket.TicketId.ToString()
                        : ticket.TicketShortCode,
                    Title = $"{ticketTypeName} request at {buildingName}",
                    Category = ticketTypeName,
                    Priority = ticket.PriorityCode?.PriorityName ?? "Normal",
                    Status = ticket.StatusCode?.StatusName ?? "Submitted",
                    SubmittedBy = requestedByName,
                    CreatedDate = created == default ? DateTime.UtcNow : created,
                    DueDate = null
                };
            }).ToList();

            TotalTickets = ticketsData.Count;
            PendingTickets = ticketsData.Count(ticket => ticket.StatusCode?.StatusName == TicketStatusNames.Submitted);
            InProgressTickets = ticketsData.Count(ticket => ticket.StatusCode?.StatusName == TicketStatusNames.InProgress);
            CompletedToday = ticketsData.Count(ticket => ticket.StatusCode?.StatusName == TicketStatusNames.Completed &&
                flows.Where(flow => flow.TicketId == ticket.TicketId)
                    .OrderByDescending(flow => flow.TimeStamp)
                    .Select(flow => flow.TimeStamp)
                    .FirstOrDefault()
                    .Date == DateTime.UtcNow.Date);

            RecentTickets = Tickets
                .OrderByDescending(ticket => ticket.CreatedDate)
                .Take(10)
                .ToList();

            UpcomingAppointments = Tickets
                .OrderByDescending(ticket => ticket.CreatedDate)
                .Take(4)
                .Select(ticket => new DashboardAppointment
                {
                    Title = ticket.Title,
                    Category = ticket.Category,
                    When = ticket.CreatedDate,
                    Status = ticket.Status
                })
                .ToList();

            var statusLookup = statusCodes.ToDictionary(code => code.Code, code => code.StatusName);

            RecentActivity = flows
                .OrderByDescending(flow => flow.TimeStamp)
                .Take(6)
                .Select(flow =>
                {
                    var ticket = ticketsData.FirstOrDefault(item => item.TicketId == flow.TicketId);
                    var ticketTypeName = ticket?.TicketType?.TypeName ?? "Maintenance";
                    var buildingName = ticket?.Building?.LocationName ?? "Unknown building";
                    var statusName = statusLookup.TryGetValue(flow.NewTicketStatus, out var name)
                        ? name
                        : "Updated";
                    var assigneeName = profileLookup.TryGetValue(flow.NewAssignee, out var profile)
                        ? $"{profile.FName} {profile.LName}".Trim()
                        : "Unassigned";

                    return new DashboardActivityItem
                    {
                        Title = $"{ticketTypeName} · {buildingName}",
                        Status = statusName,
                        Meta = assigneeName,
                        TimeStamp = flow.TimeStamp
                    };
                })
                .ToList();

            Announcements = new List<DashboardAnnouncement>
            {
                new DashboardAnnouncement
                {
                    Title = "Emergency request protocol",
                    Body = "Use High priority for safety-related issues."
                },
                new DashboardAnnouncement
                {
                    Title = "Maintenance hours",
                    Body = "Standard service runs 8am - 6pm weekdays."
                }
            };
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            // Add logout logic here
            // await HttpContext.SignOutAsync();
            return RedirectToPage("/Account/Login");
        }
    }

    // ViewModel for tickets
    public class TicketViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string SubmittedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class DashboardAppointment
    {
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime When { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class DashboardActivityItem
    {
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Meta { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; }
    }

    public class DashboardAnnouncement
    {
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
