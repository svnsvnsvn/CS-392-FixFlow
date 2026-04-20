using fixflow.web.Data;
using fixflow.web.Domain.Enums;
using fixflow.web.Dto;
using fixflow.web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using System.Linq;

namespace fixflow.web.Pages.Tickets
{
    public class DetailsModel : AppPageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITicketService _ticketService;
        private readonly IAdminService _adminService;
        private readonly IAiService _aiService;

        public DetailsModel(UserManager<AppUser> userManager, ITicketService ticketService, IAdminService adminService, IAiService aiService)
        {
            _userManager = userManager;
            _ticketService = ticketService;
            _adminService = adminService;
            _aiService = aiService;
        }

        public string UserRole { get; set; } = "Client";
        public bool IsOwnTicket { get; set; } = false;
        public TicketDetailViewModel Ticket { get; set; } = new();
        public List<NoteDto> TicketNotes { get; set; } = new();
        public NoteDto AISummary { get; set; } = new();
        public List<ActivityViewModel> ActivityHistory { get; set; } = new();
        public List<SelectListItem> AvailableTechnicians { get; set; } = new();
        public List<SelectListItem> AvailableStatuses { get; set; } = new();
        public List<SelectListItem> AvailablePriorities { get; set; } = new();

        [BindProperty]
        public string SelectedTechnicianId { get; set; } = string.Empty;

        [BindProperty]
        public int SelectedStatusCode { get; set; }

        [BindProperty]
        public int SelectedPriorityCode { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            // Get logged in user data
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Page();
            }
        
            // Get users role
            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null)
            {
                return Page();
            }
            var currentRole = roles.FirstOrDefault() ?? RoleTypes.Resident.ToString();
            RoleTypes userRole = Enum.Parse<RoleTypes>(currentRole);
            UserRole = MapRoleForUi(currentRole);

            var ticketResult = await _ticketService.GetTicketByIdentifier(id);
            var ticket = ticketResult.Success ? ticketResult.Data : null;
            if (ticket == null)
            {
                return NotFound();
            }

            var flowsResult = await _ticketService.GetTicketFlows(ticket.TicketId);
            var flows = flowsResult.Success && flowsResult.Data != null
                ? flowsResult.Data
                : new List<FfTicketFlow>();

            var statusCodeMapResult = await _ticketService.GetStatusCodeNameMap();
            var statusCodes = statusCodeMapResult.Success && statusCodeMapResult.Data != null
                ? statusCodeMapResult.Data
                : new Dictionary<int, string>();

            var profileIds = new List<string> { ticket.RequestedBy };
            profileIds.AddRange(flows.Select(f => f.NewAssignee));
            var profilesResult = await _adminService.GetUserProfilesByIds(profileIds
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList());
            var profiles = profilesResult.Success && profilesResult.Data != null
                ? profilesResult.Data.ToDictionary(profile => profile.FfUserId, profile => profile)
                : new Dictionary<string, FfUserProfile>();

            var createdDate = flows.Select(flow => flow.TimeStamp).FirstOrDefault();
            var lastFlow = flows.LastOrDefault();

            var submittedByName = profiles.TryGetValue(ticket.RequestedBy, out var requester)
                ? $"{requester.FName} {requester.LName}".Trim()
                : ticket.RequestedBy;

            var assigneeName = lastFlow != null && profiles.TryGetValue(lastFlow.NewAssignee, out var assignee)
                ? $"{assignee.FName} {assignee.LName}".Trim()
                : null;

            Ticket = new TicketDetailViewModel
            {
                Id = ticket.TicketId.ToString(),
                DisplayId = string.IsNullOrWhiteSpace(ticket.TicketShortCode) ? ticket.TicketId.ToString() : ticket.TicketShortCode,
                Title = ticket.TicketType?.TypeName ?? "Maintenance request",
                Description = string.IsNullOrWhiteSpace(ticket.TicketDescription)
                    ? "Details will appear once the request is fully documented."
                    : ticket.TicketDescription,
                Status = ticket.StatusCode?.StatusName ?? "Submitted",
                Priority = ticket.PriorityCode?.PriorityName ?? "Normal",
                Category = ticket.TicketType?.TypeName ?? "General",
                Building = ticket.Building?.LocationName ?? "Unknown building",
                RoomNumber = ticket.Unit > 0 ? ticket.Unit.ToString() : "-",
                SubmittedBy = string.IsNullOrWhiteSpace(submittedByName) ? "Unknown" : submittedByName,
                CreatedDate = createdDate == default ? DateTime.MinValue : createdDate,
                AssignedTo = assigneeName,
                CurrentStatusCode = ticket.TicketStatus,
                CurrentPriorityCode = ticket.TicketPriority,
                CurrentAssigneeId = lastFlow?.NewAssignee ?? string.Empty,
                DueDate = null,
                CompletedDate = TicketStatusIsCompleted(ticket.StatusCode?.StatusName)
                    ? (lastFlow?.TimeStamp ?? DateTime.UtcNow)
                    : null
            };

            // Get all notes for ticket and assign to Model.TicketNotes
            bool getPrivateNotes = false;
            if ((LoggedInUser.Role != RoleTypes.Resident) && (LoggedInUser.Role != RoleTypes.Pending))
            {
                getPrivateNotes = true;
            }
            var ticketNotesResult = await _ticketService.GetAllNotes(LoggedInUser, ticket.TicketId, getPrivateNotes);
            
            List<NoteDto> notesForSummary = (ticketNotesResult?.Data ?? new List<NoteDto>())!;
            TicketNotes = notesForSummary;

            if (TempData["GetAISummary"] is not null)
            {
                var aiSummary = await _aiService.GetSummaryOfNotes(notesForSummary!);
                AISummary = aiSummary?.Data ?? new NoteDto();
            }

            ActivityHistory = flows
                .OrderByDescending(flow => flow.TimeStamp)
                .Select(flow => new ActivityViewModel
                {
                    Action = statusCodes.TryGetValue(flow.NewTicketStatus, out var statusName)
                        ? $"Status set to '{statusName}'"
                        : "Ticket updated",
                    PerformedBy = profiles.TryGetValue(flow.NewAssignee, out var actor)
                        ? $"{actor.FName} {actor.LName}".Trim()
                        : "System",
                    Timestamp = flow.TimeStamp
                })
                .ToList();

            // Check if current user owns this ticket (for client view)
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IsOwnTicket = (UserRole == "Client" && !string.IsNullOrEmpty(currentUserId) && ticket.RequestedBy == currentUserId);

            // Load available technicians for assignment (only for managers)
            if (UserRole == "Manager" || UserRole == "Admin" || UserRole == "Technician")
            {
                var technicians = await _userManager.GetUsersInRoleAsync(RoleTypes.Employee.ToString());
                var techProfilesResult = await _adminService.GetUserProfilesByIds(technicians.Select(t => t.Id).ToList());
                var techniciansWithProfiles = techProfilesResult.Success && techProfilesResult.Data != null
                    ? techProfilesResult.Data
                    : new List<FfUserProfile>();

                AvailableTechnicians = techniciansWithProfiles
                    .Select(p => new SelectListItem
                    {
                        Value = p.FfUserId,
                        Text = $"{p.FName} {p.LName}".Trim()
                    })
                    .OrderBy(t => t.Text)
                    .ToList();

                AvailableTechnicians.Insert(0, new SelectListItem { Value = "", Text = "-- Select Technician --" });
            }

            if (CanManageStatus(UserRole))
            {
                await LoadAvailableStatusesAsync(Ticket.CurrentStatusCode);
                await LoadAvailablePrioritiesAsync(Ticket.CurrentPriorityCode);
            }

            return Page();
        }

        private static bool TicketStatusIsCompleted(string? statusName)
        {
            return string.Equals(statusName, "Completed", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<IActionResult> OnPostAddCommentAsync(string ticketId, string commentText, bool internalOnly)
        {
            if (string.IsNullOrWhiteSpace(commentText))
            {
                return RedirectToPage(new { id = ticketId });
            }

            // Create new note
            NoteDto newNote = new NoteDto();
            newNote.NoteText = commentText;
            newNote.TicketId = new Guid(ticketId);
            newNote.InternalOnly = internalOnly;
            newNote.EnteredByUserId = LoggedInUser.UserId;
            newNote.TimeStamp = DateTime.UtcNow;

            // Write Note to Db
            var result = await _ticketService.AddNewNote(LoggedInUser, newNote);

            return RedirectToPage(new { id = ticketId });
        }

        public async Task<IActionResult> OnPostSummarizeCommentsAsync(string ticketId)
        {
            TempData["GetAISummary"] = true;
            return RedirectToPage(new { id = ticketId });
        }

        public async Task<IActionResult> OnPostAssignTechnicianAsync(string ticketId)
        {
            if (string.IsNullOrWhiteSpace(SelectedTechnicianId))
            {
                return RedirectToPage(new { id = ticketId });
            }

            // Parse ticket ID
            var ticketResult = await _ticketService.GetTicketByIdentifier(ticketId);
            var ticket = ticketResult.Success ? ticketResult.Data : null;

            if (ticket == null)
            {
                return NotFound();
            }

            // Get current user
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Determine user role
            RoleTypes userRole = RoleTypes.Manager;
            if (User.IsInRole(RoleTypes.Admin.ToString()))
                userRole = RoleTypes.Admin;

            // Get "Assigned" status code
            var assignedCodeResult = await _ticketService.GetStatusCode("Assigned");
            if (!assignedCodeResult.Success)
            {
                TempData["ErrorMessage"] = "System configuration error: Assigned status not found.";
                return RedirectToPage(new { id = ticketId });
            }

            // Use the real backend method: ReassignTicket
            var result = await _ticketService.ReassignTicket(
                currentUser.Id,
                userRole,
                ticket.TicketId,
                SelectedTechnicianId,
                assignedCodeResult.Data
            );

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Error ?? "Ticket assignment failed.";
            }
            else
            {
                TempData["SuccessMessage"] = "Ticket assigned successfully!";
            }

            return RedirectToPage(new { id = ticketId });
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(string ticketId)
        {
            if (!CanManageStatus(UserRoleFromClaims()))
            {
                return Forbid();
            }

            var ticketResult = await _ticketService.GetTicketByIdentifier(ticketId);
            var ticket = ticketResult.Success ? ticketResult.Data : null;
            if (ticket == null)
            {
                return NotFound();
            }

            var flowsResult = await _ticketService.GetTicketFlows(ticket.TicketId);
            var currentAssigneeId = flowsResult.Success && flowsResult.Data != null
                ? flowsResult.Data.OrderByDescending(flow => flow.TimeStamp).Select(flow => flow.NewAssignee).FirstOrDefault()
                : null;

            if (string.IsNullOrWhiteSpace(currentAssigneeId))
            {
                TempData["ErrorMessage"] = "Assign a technician before changing status.";
                return RedirectToPage(new { id = ticketId });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var requestorRole = GetStaffRoleFromClaims();
            if (requestorRole == null)
            {
                return Forbid();
            }

            var result = await _ticketService.ReassignTicket(
                currentUser.Id,
                requestorRole.Value,
                ticket.TicketId,
                currentAssigneeId,
                SelectedStatusCode);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Error ?? "Status update failed.";
            }
            else
            {
                TempData["SuccessMessage"] = "Status updated successfully.";
            }

            return RedirectToPage(new { id = ticketId });
        }

        public async Task<IActionResult> OnPostUpdatePriorityAsync(string ticketId)
        {
            if (!CanManageStatus(UserRoleFromClaims()))
            {
                return Forbid();
            }

            var ticketResult = await _ticketService.GetTicketByIdentifier(ticketId);
            var ticket = ticketResult.Success ? ticketResult.Data : null;
            if (ticket == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var requestorRole = GetStaffRoleFromClaims();
            if (requestorRole == null)
            {
                return Forbid();
            }

            var updateDto = new TicketDataDto
            {
                TicketId = ticket.TicketId,
                RequestedBy = ticket.RequestedBy,
                Location = ticket.Location,
                Unit = ticket.Unit,
                TicketTroubleType = ticket.TicketTroubleType,
                TicketPriority = SelectedPriorityCode,
                TicketSubject = ticket.TicketSubject,
                TicketDescription = ticket.TicketDescription
            };

            var result = await _ticketService.UpdateTicket(currentUser.Id, requestorRole.Value, updateDto);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Error ?? "Priority update failed.";
            }
            else
            {
                TempData["SuccessMessage"] = "Priority updated successfully.";
            }

            return RedirectToPage(new { id = ticketId });
        }

        private async Task LoadAvailableStatusesAsync(int selectedStatusCode)
        {
            var statusResult = await _ticketService.GetStatusCodeList();
            AvailableStatuses = statusResult.Success && statusResult.Data != null
                ? statusResult.Data
                    .Where(status => status.StatusCode.HasValue && !string.IsNullOrWhiteSpace(status.StatusName))
                    .OrderBy(status => status.StatusCode)
                    .Select(status => new SelectListItem
                    {
                        Value = status.StatusCode!.Value.ToString(),
                        Text = status.StatusName!
                    })
                    .ToList()
                : new List<SelectListItem>();

            SelectedStatusCode = selectedStatusCode;
        }

        private async Task LoadAvailablePrioritiesAsync(int selectedPriorityCode)
        {
            var priorityResult = await _adminService.GetPriorityCodeList();
            AvailablePriorities = priorityResult.Success && priorityResult.Data != null
                ? priorityResult.Data
                    .Where(priority => !string.IsNullOrWhiteSpace(priority.PriorityName))
                    .OrderBy(priority => priority.PriorityCode)
                    .Select(priority => new SelectListItem
                    {
                        Value = priority.Id.ToString(),
                        Text = priority.PriorityName!
                    })
                    .ToList()
                : new List<SelectListItem>();

            SelectedPriorityCode = selectedPriorityCode;
        }

        private static bool CanManageStatus(string role)
        {
            return role == "Admin" || role == "Manager" || role == "Technician";
        }

        private string UserRoleFromClaims()
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

            return "Client";
        }

        private RoleTypes? GetStaffRoleFromClaims()
        {
            if (User.IsInRole(RoleTypes.Admin.ToString()))
            {
                return RoleTypes.Admin;
            }

            if (User.IsInRole(RoleTypes.Manager.ToString()))
            {
                return RoleTypes.Manager;
            }

            if (User.IsInRole(RoleTypes.Employee.ToString()))
            {
                return RoleTypes.Employee;
            }

            return null;
        }

        public async Task<IActionResult> OnPostStartWorkAsync(Guid ticketId)
        {
            await _ticketService.ReassignTicket(LoggedInUser.UserId,(RoleTypes)LoggedInUser.Role, ticketId,LoggedInUser.UserId,_ticketService.GetStatusCode("In Progress").Result.Data);

            return RedirectToPage(new { ticketId });
        }

        // ViewModels
        public class TicketDetailViewModel
        {
            public string Id { get; set; } = string.Empty;
            public string DisplayId { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string Priority { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string Building { get; set; } = string.Empty;
            public string RoomNumber { get; set; } = string.Empty;
            public string SubmittedBy { get; set; } = string.Empty;
            public string? AssignedTo { get; set; }
            public int CurrentStatusCode { get; set; }
            public int CurrentPriorityCode { get; set; }
            public string CurrentAssigneeId { get; set; } = string.Empty;
            public DateTime CreatedDate { get; set; }
            public DateTime? DueDate { get; set; }
            public DateTime? CompletedDate { get; set; }
        }

        public class CommentViewModel
        {
            public string AuthorName { get; set; } = string.Empty;
            public string AuthorRole { get; set; } = string.Empty;
            public string Text { get; set; } = string.Empty;
            public DateTime CreatedDate { get; set; }
            public bool IsInternal { get; set; }
        }

        public class ActivityViewModel
        {
            public string Action { get; set; } = string.Empty;
            public string PerformedBy { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; }
        }

        private static string MapRoleForUi(string role)
        {
            return role switch
            {
                nameof(RoleTypes.Admin) => "Admin",
                nameof(RoleTypes.Manager) => "Manager",
                nameof(RoleTypes.Employee) => "Technician",
                nameof(RoleTypes.Resident) => "Client",
                nameof(RoleTypes.Pending) => "Client",
                _ => "Client"
            };
        }
    }
}
