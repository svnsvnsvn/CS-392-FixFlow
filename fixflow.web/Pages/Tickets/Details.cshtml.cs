using fixflow.web.Data;
using fixflow.web.Domain.Enums;
using fixflow.web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace fixflow.web.Pages.Tickets
{
    public class DetailsModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITicketService _ticketService;
        private readonly IAdminService _adminService;

        public DetailsModel(UserManager<AppUser> userManager, ITicketService ticketService, IAdminService adminService)
        {
            _userManager = userManager;
            _ticketService = ticketService;
            _adminService = adminService;
        }

        public string UserRole { get; set; } = "Client";
        public bool IsOwnTicket { get; set; } = false;
        public TicketDetailViewModel Ticket { get; set; } = new();
        public List<CommentViewModel> PublicComments { get; set; } = new();
        public List<CommentViewModel> InternalNotes { get; set; } = new();
        public List<ActivityViewModel> ActivityHistory { get; set; } = new();
        public List<SelectListItem> AvailableTechnicians { get; set; } = new();
        public string CommentSummaryStub { get; set; } = string.Empty;

        [BindProperty]
        public string SelectedTechnicianId { get; set; } = string.Empty;

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
                Id = string.IsNullOrWhiteSpace(ticket.TicketShortCode)
                    ? ticket.TicketId.ToString()
                    : ticket.TicketShortCode,
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
                DueDate = null,
                CompletedDate = TicketStatusIsCompleted(ticket.StatusCode?.StatusName)
                    ? (lastFlow?.TimeStamp ?? DateTime.UtcNow)
                    : null
            };

            var externalResult = await _ticketService.GetExternalNotes(ticket.TicketId);
            var externalNotes = externalResult.Success && externalResult.Data != null
                ? externalResult.Data
                : new List<FfExternalNotes>();
            PublicComments = externalNotes
                .Select(note => new CommentViewModel
                {
                    AuthorName = note.CreatedBy,
                    AuthorRole = string.Empty,
                    Text = note.Content,
                    CreatedDate = note.TimeStamp,
                    IsInternal = false
                })
                .ToList();

            var internalResult = await _ticketService.GetInternalNotes(ticket.TicketId);
            var internalNotes = internalResult.Success && internalResult.Data != null
                ? internalResult.Data
                : new List<FfInternalNotes>();
            InternalNotes = internalNotes
                .Select(note => new CommentViewModel
                {
                    AuthorName = note.CreatedBy,
                    AuthorRole = "Staff",
                    Text = note.Content,
                    CreatedDate = note.TimeStamp,
                    IsInternal = true
                })
                .ToList();

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

            if (TempData.TryGetValue("CommentSummaryStub", out var summaryStubObj))
            {
                CommentSummaryStub = summaryStubObj?.ToString() ?? string.Empty;
            }

            // Check if current user owns this ticket (for client view)
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IsOwnTicket = (UserRole == "Client" && !string.IsNullOrEmpty(currentUserId) && ticket.RequestedBy == currentUserId);

            // Load available technicians for assignment (only for managers)
            if (UserRole == "Manager" || UserRole == "Admin")
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

            return Page();
        }

        private static bool TicketStatusIsCompleted(string? statusName)
        {
            return string.Equals(statusName, "Completed", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<IActionResult> OnPostAddCommentAsync(string ticketId, string commentText)
        {
            if (string.IsNullOrWhiteSpace(commentText))
            {
                return RedirectToPage(new { id = ticketId });
            }

            // Backend will add comment to database:
            // var comment = new TicketComment
            // {
            //     TicketId = ticketId,
            //     UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            //     CommentText = commentText,
            //     IsInternalNote = false,
            //     CreatedDate = DateTime.UtcNow
            // };
            // await ticketService.AddExternalComment(...);

            // Also add to activity history:
            // var activity = new TicketHistory
            // {
            //     TicketId = ticketId,
            //     ChangedById = User.FindFirstValue(ClaimTypes.NameIdentifier),
            //     ChangeType = "Comment Added",
            //     Notes = commentText.Substring(0, Math.Min(100, commentText.Length)),
            //     ChangedDate = DateTime.UtcNow
            // };

            return RedirectToPage(new { id = ticketId });
        }

        public IActionResult OnPostSummarizeCommentsAsync(string ticketId)
        {
            // TODO(Adam): Implement summary generation by querying these entities:
            // 1) FfExternalNotess (customer-visible comments): TicketId, Content, CreatedBy, TimeStamp.
            // 2) FfInternalNotess (staff-only context): TicketId, Content, CreatedBy, TimeStamp.
            // 3) FfTicketFlows (status/activity timeline): TicketId, NewTicketStatus, NewAssignee, TimeStamp.
            //
            // Suggested flow:
            // - Fetch notes for the provided TicketId, sorted ascending by TimeStamp.
            // - Optionally enrich names by joining CreatedBy/NewAssignee to FfUserProfiles.FfUserId.
            // - Build an input transcript with sections [Public Comments], [Internal Notes], [Ticket Activity].
            // - Call your preferred summarization service (LLM/provider) and store result in a summary table
            //   or cache field (e.g., TicketId + GeneratedAt + SummaryText + ModelVersion).
            // - Return summary text to this page model (CommentSummaryStub), and gate internal content by role.
            TempData["CommentSummaryStub"] =
                "Stub preview: summary generation is wired at UI level. Adam should implement server-side aggregation from FfExternalNotess, FfInternalNotess, and FfTicketFlows for this ticket, then return concise highlights, blockers, and next actions.";
            return RedirectToPage(new { id = ticketId });
        }

        public async Task<IActionResult> OnPostAddInternalNoteAsync(string ticketId, string noteText)
        {
            if (string.IsNullOrWhiteSpace(noteText))
            {
                return RedirectToPage(new { id = ticketId });
            }

            // Backend will add internal note to database:
            // var note = new TicketComment
            // {
            //     TicketId = ticketId,
            //     UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            //     CommentText = noteText,
            //     IsInternalNote = true,  // This is the key difference!
            //     CreatedDate = DateTime.UtcNow
            // };
            // await ticketService.AddInternalNote(...);

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

        // ViewModels
        public class TicketDetailViewModel
        {
            public string Id { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string Priority { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string Building { get; set; } = string.Empty;
            public string RoomNumber { get; set; } = string.Empty;
            public string SubmittedBy { get; set; } = string.Empty;
            public string? AssignedTo { get; set; }
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
