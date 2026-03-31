using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using fixflow.web.Domain.Enums;
using fixflow.web.Services;


namespace fixflow.web.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly ITicketService _ticketService;

        public DashboardModel(ITicketService ticketService)
        {
            _ticketService = ticketService;
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

        // Tickets list (full set for the table; UI paginates 20 per page)
        public List<TicketViewModel> Tickets { get; set; } = new();

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
            if (User.IsInRole(RoleTypes.Admin.ToString()))
            {
                return "Admin";
            }

            if (User.IsInRole(RoleTypes.Manager.ToString()))
            {
                return "Manager";
            }

            if (User.IsInRole(RoleTypes.Employee.ToString()))
            {
                return "Technician";
            }

            if (User.IsInRole(RoleTypes.Resident.ToString()) || User.IsInRole(RoleTypes.Pending.ToString()))
            {
                return "Client";
            }

            return "Client";
        }

        private async Task BuildDashboardFromDbAsync(string role)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var roleType = role switch
            {
                "Admin" => RoleTypes.Admin,
                "Manager" => RoleTypes.Manager,
                "Technician" => RoleTypes.Employee,
                _ => RoleTypes.Resident
            };

            var bundleResult = await _ticketService.GetDashboardBundle(userId, roleType);
            if (!bundleResult.Success || bundleResult.Data == null)
            {
                Tickets = new List<TicketViewModel>();
                UpcomingAppointments = new List<DashboardAppointment>();
                RecentActivity = new List<DashboardActivityItem>();
                Announcements = new List<DashboardAnnouncement>();
                return;
            }

            var ticketsData = bundleResult.Data.Tickets;
            var flows = bundleResult.Data.Flows;
            var profileLookup = bundleResult.Data.ProfileDisplayNames;

            Tickets = ticketsData.Select(ticket =>
            {
                var created = flows.Where(flow => flow.TicketId == ticket.TicketId)
                    .OrderBy(flow => flow.TimeStamp)
                    .Select(flow => flow.TimeStamp)
                    .FirstOrDefault();

                var requestedByName = profileLookup.TryGetValue(ticket.RequestedBy, out var profileName)
                    ? profileName
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
            })
                .OrderByDescending(ticket => ticket.CreatedDate)
                .ToList();

            var result = await _ticketService.GetStatusCode("Submitted");
            int pendingCode = result.Data;

            result = await _ticketService.GetStatusCode("Completed");
            int completedCode = result.Data;

            result = await _ticketService.GetStatusCode("In Progress");
            int inProgressCode = result.Data;

            TotalTickets = ticketsData.Count;
            PendingTickets = ticketsData.Count(ticket => ticket.TicketStatus == pendingCode);
            InProgressTickets = ticketsData.Count(ticket => ticket.TicketStatus == inProgressCode);
            CompletedToday = ticketsData.Count(ticket => ticket.TicketStatus == completedCode &&
                flows.Where(flow => flow.TicketId == ticket.TicketId)
                    .OrderByDescending(flow => flow.TimeStamp)
                    .Select(flow => flow.TimeStamp)
                    .FirstOrDefault()
                    .Date == DateTime.UtcNow.Date);

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

            var statusLookup = bundleResult.Data.StatusCodeNames;

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
                    var assigneeName = profileLookup.TryGetValue(flow.NewAssignee, out var assigneeProfileName)
                        ? assigneeProfileName
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

        public IActionResult OnPostLogout()
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
