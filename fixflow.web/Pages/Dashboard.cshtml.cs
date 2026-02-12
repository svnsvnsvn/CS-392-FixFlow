using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace fixflow.web.Pages
{
    public class DashboardModel : PageModel
    {
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

        // Tickets list
        public List<TicketViewModel> Tickets { get; set; } = new();

        public IActionResult OnGet(string? role = null)
        {
            // Get user info (from authentication)
            UserName = User.Identity?.Name ?? "Demo User";
            
            // FOR DEMO: Allow testing different roles via query string
            // In production, this would come from: User.IsInRole("Admin"), etc.
            UserRole = role ?? "Client"; // Default to Client for testing
            
            // Set user initials
            UserInitials = GetInitials(UserName);
            
            // Set welcome message and data based on role
            SetupRoleBasedContent();
            
            return Page();
        }

        private void SetupRoleBasedContent()
        {
            switch (UserRole)
            {
                case "Client":
                    SetupClientView();
                    break;
                case "Technician":
                    SetupTechnicianView();
                    break;
                case "Manager":
                    SetupManagerView();
                    break;
                case "Admin":
                    SetupAdminView();
                    break;
                default:
                    SetupClientView();
                    break;
            }
        }

        private void SetupClientView()
        {
            WelcomeMessage = "Track your maintenance requests and submit new tickets.";
            TicketsSectionTitle = "My Tickets";
            
            // Mock client tickets
            Tickets = new List<TicketViewModel>
            {
                new TicketViewModel
                {
                    Id = "001",
                    Title = "Leaking faucet in kitchen",
                    Category = "Plumbing",
                    Priority = "High",
                    Status = "In Progress",
                    SubmittedBy = UserName,
                    CreatedDate = DateTime.Now.AddDays(-2),
                    DueDate = DateTime.Now.AddDays(1)
                },
                new TicketViewModel
                {
                    Id = "005",
                    Title = "AC not cooling properly",
                    Category = "HVAC",
                    Priority = "High",
                    Status = "Assigned",
                    SubmittedBy = UserName,
                    CreatedDate = DateTime.Now.AddDays(-1)
                },
                new TicketViewModel
                {
                    Id = "012",
                    Title = "Light bulb replacement",
                    Category = "Electrical",
                    Priority = "Low",
                    Status = "Completed",
                    SubmittedBy = UserName,
                    CreatedDate = DateTime.Now.AddDays(-5),
                    DueDate = DateTime.Now.AddDays(-2)
                }
            };
        }

        private void SetupTechnicianView()
        {
            WelcomeMessage = "You have 5 assigned tasks. 2 are due today.";
            TicketsSectionTitle = "My Assignments";
            
            // Mock technician assigned tickets
            Tickets = new List<TicketViewModel>
            {
                new TicketViewModel
                {
                    Id = "001",
                    Title = "Leaking faucet in Building A",
                    Category = "Plumbing",
                    Priority = "High",
                    Status = "In Progress",
                    SubmittedBy = "john.doe",
                    CreatedDate = DateTime.Now.AddHours(-3),
                    DueDate = DateTime.Now.AddHours(5)
                },
                new TicketViewModel
                {
                    Id = "003",
                    Title = "Replace ceiling tiles - Room 205",
                    Category = "Maintenance",
                    Priority = "Medium",
                    Status = "Assigned",
                    SubmittedBy = "jane.smith",
                    CreatedDate = DateTime.Now.AddDays(-1),
                    DueDate = DateTime.Now.AddDays(2)
                },
                new TicketViewModel
                {
                    Id = "007",
                    Title = "Fix door lock - Office 301",
                    Category = "Maintenance",
                    Priority = "High",
                    Status = "Assigned",
                    SubmittedBy = "mike.johnson",
                    CreatedDate = DateTime.Now.AddDays(-2),
                    DueDate = DateTime.Now
                }
            };
        }

        private void SetupManagerView()
        {
            WelcomeMessage = "Manage tickets, assign technicians, and oversee operations.";
            TicketsSectionTitle = "All Tickets";
            
            // Stats for managers
            TotalTickets = 47;
            PendingTickets = 12;
            InProgressTickets = 18;
            CompletedToday = 7;
            
            // Mock all tickets overview
            Tickets = new List<TicketViewModel>
            {
                new TicketViewModel
                {
                    Id = "015",
                    Title = "Emergency plumbing repair",
                    Category = "Plumbing",
                    Priority = "High",
                    Status = "Submitted",
                    SubmittedBy = "resident.a",
                    CreatedDate = DateTime.Now.AddMinutes(-30)
                },
                new TicketViewModel
                {
                    Id = "014",
                    Title = "HVAC maintenance check",
                    Category = "HVAC",
                    Priority = "Medium",
                    Status = "In Review",
                    SubmittedBy = "resident.b",
                    CreatedDate = DateTime.Now.AddHours(-2)
                },
                new TicketViewModel
                {
                    Id = "013",
                    Title = "Electrical outlet not working",
                    Category = "Electrical",
                    Priority = "High",
                    Status = "Assigned",
                    SubmittedBy = "resident.c",
                    CreatedDate = DateTime.Now.AddHours(-5)
                },
                new TicketViewModel
                {
                    Id = "012",
                    Title = "Weekly cleaning service",
                    Category = "Cleaning",
                    Priority = "Low",
                    Status = "In Progress",
                    SubmittedBy = "manager",
                    CreatedDate = DateTime.Now.AddDays(-1)
                }
            };
        }

        private void SetupAdminView()
        {
            WelcomeMessage = "System overview and administrative controls.";
            TicketsSectionTitle = "Recent Tickets";
            
            // Stats for admin
            TotalTickets = 247;
            PendingTickets = 23;
            InProgressTickets = 45;
            CompletedToday = 15;
            
            // Mock recent system tickets
            Tickets = new List<TicketViewModel>
            {
                new TicketViewModel
                {
                    Id = "020",
                    Title = "Fire alarm inspection",
                    Category = "Safety",
                    Priority = "High",
                    Status = "Submitted",
                    SubmittedBy = "safety.officer",
                    CreatedDate = DateTime.Now.AddMinutes(-15)
                },
                new TicketViewModel
                {
                    Id = "019",
                    Title = "Roof leak - Building C",
                    Category = "Maintenance",
                    Priority = "High",
                    Status = "In Review",
                    SubmittedBy = "manager.1",
                    CreatedDate = DateTime.Now.AddHours(-1)
                },
                new TicketViewModel
                {
                    Id = "018",
                    Title = "Parking lot lighting",
                    Category = "Electrical",
                    Priority = "Medium",
                    Status = "Assigned",
                    SubmittedBy = "security",
                    CreatedDate = DateTime.Now.AddHours(-4)
                },
                new TicketViewModel
                {
                    Id = "017",
                    Title = "Monthly pest control",
                    Category = "Maintenance",
                    Priority = "Low",
                    Status = "Completed",
                    SubmittedBy = "facility.manager",
                    CreatedDate = DateTime.Now.AddDays(-2)
                }
            };
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
}
